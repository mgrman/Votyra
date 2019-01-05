using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine;
using Votyra.Core.GroupSelectors;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Logging;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainGenerators;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core
{
    //TODO: move to floats
    public class TerrainGeneratorManager2i : IDisposable
    {
        private readonly IFrameDataProvider2i _frameDataProvider;
        private readonly IThreadSafeLogger _logger;
        private readonly IProfiler _profiler;
        private readonly IStateModel _stateModel;
        private readonly ITerrainConfig _terrainConfig;
        private readonly ITerrainVertexPostProcessor _vertexPostProcessor;
        private readonly ITerrainUVPostProcessor _uvPostProcessor;
        private readonly IInterpolationConfig _interpolationConfig;
        private readonly Func<GameObject> _gameObjectFactory;
        
        private CancellationTokenSource _onDestroyCts = new CancellationTokenSource();
        private SetDictionary<Vector2i, GameObject> _meshFilters = new SetDictionary<Vector2i, GameObject>();
        private HashSet<Vector2i> _skippedAreas = new HashSet<Vector2i>();

        public TerrainGeneratorManager2i(Func<GameObject> gameObjectFactory, IThreadSafeLogger logger, ITerrainConfig terrainConfig, IStateModel stateModel, IProfiler profiler, IFrameDataProvider2i frameDataProvider, [InjectOptional] ITerrainVertexPostProcessor vertexPostProcessor, [InjectOptional] ITerrainUVPostProcessor uvPostProcessor, IInterpolationConfig interpolationConfig)
        {
            _gameObjectFactory = gameObjectFactory;
            _logger = logger;
            _terrainConfig = terrainConfig;
            _stateModel = stateModel;
            _profiler = profiler;
            _frameDataProvider = frameDataProvider;
            _vertexPostProcessor = vertexPostProcessor;
            _uvPostProcessor = uvPostProcessor;
            _interpolationConfig = interpolationConfig;

            StartUpdateing();
        }

        public void Dispose()
        {
            _onDestroyCts.Cancel();
        }

        private async void StartUpdateing()
        {
            while (!_onDestroyCts.IsCancellationRequested)
            {
#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying)
                    return;
#endif
                try
                {
                    if (_stateModel.IsEnabled)
                    {
                        var context = _frameDataProvider.GetCurrentFrameData(_meshFilters, _skippedAreas, (_interpolationConfig.ActiveAlgorithm == IntepolationAlgorithm.Cubic && _interpolationConfig.MeshSubdivision != 1) ? 2 : 1);
                        await UpdateTerrain(context, _terrainConfig.Async, _onDestroyCts.Token);
                    }
                    else
                    {
                        await UniTask.Delay(10);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex);
                    await UniTask.Delay(10);
                }
            }
        }

        private async Task UpdateTerrain(IFrameData2i context, bool async, CancellationToken token)
        {
            GroupActions<Vector2i> groupActions = null;
            IReadOnlyPooledDictionary<Vector2i, UnityMesh> results = null;
            try
            {
                Func<IReadOnlyPooledDictionary<Vector2i, UnityMesh>> computeAction = () =>
                {
                    using (_profiler.Start("Creating visible groups"))
                    {
                        groupActions = context.GetGroupsToUpdate();
                        if (groupActions.ToRecompute.Any())
                        {
                            _logger.LogMessage($"Groups to recompute {groupActions.ToRecompute.Count()}. Groups to keep {groupActions.ToKeep.Count()}.\r\nRecomputed:\r\n{groupActions.ToRecompute.StringJoin("\r\n")}");
                        }
                    }

                    var toRecompute = groupActions?.ToRecompute ?? Enumerable.Empty<Vector2i>();
                    if (toRecompute.Any())
                    {
                        using (_profiler.Start("TerrainMeshGenerator"))
                        {
                            PooledDictionary<Vector2i, UnityMesh> meshes;
                            using (_profiler.Start("init"))
                            {
                                meshes = PooledDictionary<Vector2i, UnityMesh>.Create();
                            }

                            foreach (var group in toRecompute)
                            {
                                var bounds = Area3f.FromMinAndSize((group * context.CellInGroupCount).ToVector3f(context.RangeZ.Min), context.CellInGroupCount.ToVector3f(context.RangeZ.Size));

                                IPooledTerrainMesh pooledMesh;
                                if (_interpolationConfig.DynamicMeshes)
                                {
                                    pooledMesh = PooledTerrainMeshContainer<ExpandingUnityTerrainMesh>.CreateDirty();
                                    pooledMesh.Mesh.Clear(bounds, _vertexPostProcessor == null ? (Func<Vector3f, Vector3f>) null : _vertexPostProcessor.PostProcessVertex, _uvPostProcessor == null ? (Func<Vector2f, Vector2f>) null : _uvPostProcessor.ProcessUV);
                                }
                                else
                                {
                                    var triangleCount = context.CellInGroupCount.AreaSum * 2 * _interpolationConfig.MeshSubdivision * _interpolationConfig.MeshSubdivision;
                                    pooledMesh = PooledTerrainMeshWithFixedCapacityContainer<FixedTerrainMesh2i>.CreateDirty(triangleCount);
                                    pooledMesh.Mesh.Clear(bounds, _vertexPostProcessor == null ? (Func<Vector3f, Vector3f>) null : _vertexPostProcessor.PostProcessVertex, _uvPostProcessor == null ? (Func<Vector2f, Vector2f>) null : _uvPostProcessor.ProcessUV);
                                }

                                if (_interpolationConfig.MeshSubdivision > 1 && _interpolationConfig.ActiveAlgorithm == IntepolationAlgorithm.Cubic && !_interpolationConfig.DynamicMeshes)
                                {
                                    BicubicTerrainMesher2f.GetResultingMesh(pooledMesh.Mesh, group, context.CellInGroupCount, context.Image, context.Mask, _interpolationConfig.MeshSubdivision);
                                }
                                else if (_interpolationConfig.ImageSubdivision > 1 && _interpolationConfig.MeshSubdivision == 1)
                                {
                                    DynamicTerrainMesher2f.GetResultingMesh(pooledMesh.Mesh, group, context.CellInGroupCount, context.Image, context.Mask);
                                }
                                else if (_interpolationConfig.MeshSubdivision == 1)
                                {
                                    TerrainMesher2f.GetResultingMesh(pooledMesh.Mesh, group, context.CellInGroupCount, context.Image, context.Mask);
                                }

                                pooledMesh.FinalizeMesh();
                                meshes[group] = pooledMesh.Mesh.GetUnityMesh(null);
                            }

                            return meshes;
                        }
                    }
                    else
                    {
                        return null;
                    }
                };

                if (async)
                {
                    results = await Task.Run(computeAction);
                }
                else
                {
                    results = computeAction();
                }

                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (results != null)
                {
                    using (_profiler.Start("Applying mesh"))
                    {
                        var toKeepGroups = groupActions?.ToKeep ?? ReadOnlySet<Vector2i>.Empty;

                        if (results != null)
                        {
                            using (_profiler.Start("Setting Mesh"))
                            {
                                var toDeleteGroups = _meshFilters.Keys.Except(results.Keys).Except(toKeepGroups).ToList();

                                foreach (var terrainMesh in results)
                                {
                                    var group = terrainMesh.Key;
                                    var triangleMesh = terrainMesh.Value;

                                    if (terrainMesh.Value == null || triangleMesh.VertexCount == 0)
                                    {
                                        if (_meshFilters.ContainsKey(group))
                                        {
                                            _meshFilters[group]?.Destroy();
                                        }

                                        _meshFilters[group] = null;
                                        continue;
                                    }

                                    var unityData = _meshFilters.TryGetValue(group).NullIfDestroyed() ?? _gameObjectFactory();

                                    triangleMesh.SetUnityMesh(unityData);
                                    _meshFilters[group] = unityData;
                                }

                                foreach (var toDeleteGroup in toDeleteGroups)
                                {
                                    _meshFilters[toDeleteGroup]?.Destroy();
                                    _meshFilters.Remove(toDeleteGroup);
                                }
                            }
                        }
                    }
                }

                if (!async)
                {
                    await UniTask.Yield();
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            finally
            {
                context?.Dispose();
                groupActions?.Dispose();
                results?.Dispose();
            }
        }
    }
}