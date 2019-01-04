using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine;
using Votyra.Core.GroupSelectors;
using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainGenerators;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;

namespace Votyra.Core
{
    //TODO: move to floats
    public class TerrainGeneratorManager2i : IDisposable
    {
        protected readonly IFrameDataProvider2i _frameDataProvider;
        protected readonly IThreadSafeLogger _logger;

        protected readonly IProfiler _profiler;
        protected readonly IStateModel _stateModel;
        protected readonly ITerrainConfig _terrainConfig;
        protected readonly ITerrainMesher2f _terrainMesher;

        protected readonly Func<GameObject> _gameObjectFactory;
        private CancellationTokenSource _onDestroyCts = new CancellationTokenSource();


        private SetDictionary<Vector2i, GameObject> _meshFilters = new SetDictionary<Vector2i, GameObject>();
        private HashSet<Vector2i> _skippedAreas = new HashSet<Vector2i>();

        public TerrainGeneratorManager2i(Func<GameObject> gameObjectFactory, IThreadSafeLogger logger, ITerrainConfig terrainConfig, IStateModel stateModel, ITerrainMesher2f terrainMesher, IProfiler profiler, IFrameDataProvider2i frameDataProvider)
        {
            _gameObjectFactory = gameObjectFactory;
            _logger = logger;
            _terrainConfig = terrainConfig;
            _stateModel = stateModel;
            _terrainMesher = terrainMesher;
            _profiler = profiler;
            _frameDataProvider = frameDataProvider;

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
                        var context = _frameDataProvider.GetCurrentFrameData(_meshFilters, _skippedAreas);
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
                                var mesh = _terrainMesher.GetResultingMesh(group, context.Image, context.Mask);
                                
                                meshes[group] = mesh.GetUnityMesh(null);
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