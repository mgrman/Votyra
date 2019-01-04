using System;
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
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;

namespace Votyra.Core
{
    //TODO: move to floats
    public class TerrainGeneratorManager2i : IDisposable
    {
        protected readonly IFrameDataProvider2i _frameDataProvider;
        protected readonly IGroupSelector2i _groupsSelector;
        protected readonly IThreadSafeLogger _logger;

        protected readonly IMeshUpdater _meshUpdater;
        protected readonly IProfiler _profiler;
        protected readonly IStateModel _stateModel;
        protected readonly ITerrainConfig _terrainConfig;
        protected readonly IUnityTerrainGenerator2i _terrainGenerator;
        private CancellationTokenSource _onDestroyCts = new CancellationTokenSource();


        private SetDictionary<Vector2i, GameObject> _meshFilters = new SetDictionary<Vector2i, GameObject>();

        public TerrainGeneratorManager2i(IThreadSafeLogger logger, ITerrainConfig terrainConfig, IGroupSelector2i groupsSelector, IUnityTerrainGenerator2i terrainGenerator, IMeshUpdater meshUpdater, IStateModel stateModel, IProfiler profiler, IFrameDataProvider2i frameDataProvider)
        {
            _logger = logger;
            _terrainConfig = terrainConfig;
            _groupsSelector = groupsSelector;
            _terrainGenerator = terrainGenerator;
            _meshUpdater = meshUpdater;
            _stateModel = stateModel;
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
                        var context = _frameDataProvider.GetCurrentFrameData(_meshFilters);
                        await UpdateTerrain(context,_terrainConfig.Async, _onDestroyCts.Token);
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
                        groupActions = _groupsSelector.GetGroupsToUpdate(context);
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
                                meshes[group]= _terrainGenerator.Generate(group, context.Image, context.Mask);
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
                                    var offset = (group * _terrainConfig.CellInGroupCount.XY).ToVector2f();
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

                                    var unityData = _meshFilters.TryGetValue(group);

                                    var go=_meshUpdater.UpdateMesh(triangleMesh, unityData);
                                    _meshFilters[group] = go;
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