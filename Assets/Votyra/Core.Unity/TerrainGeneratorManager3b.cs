using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UniRx.Async;
using UnityEditor;
using UnityEngine;
using Votyra.Core.GroupSelectors;
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
    public class TerrainGeneratorManager3b : IDisposable
    {
        protected readonly IFrameDataProvider3b _frameDataProvider;
        
        protected readonly Func<GameObject> _gameObjectFactory;
        
        protected readonly IGroupSelector3b _groupsSelector;
        
        protected readonly IThreadSafeLogger _logger;

        private readonly SetDictionary<Vector3i, GameObject> _meshFilters = new SetDictionary<Vector3i, GameObject>();
        
        private readonly CancellationTokenSource _onDestroyCts = new CancellationTokenSource();

        protected readonly IProfiler _profiler;
        
        protected readonly IStateModel _stateModel;
        
        protected readonly ITerrainConfig _terrainConfig;
        
        protected readonly ITerrainGenerator3b _terrainGenerator;

        public TerrainGeneratorManager3b(Func<GameObject> gameObjectFactory, IThreadSafeLogger logger, ITerrainConfig terrainConfig, IGroupSelector3b groupsSelector, ITerrainGenerator3b terrainGenerator, IStateModel stateModel, IProfiler profiler, IFrameDataProvider3b frameDataProvider)
        {
            _gameObjectFactory = gameObjectFactory;
            _logger = logger;
            _terrainConfig = terrainConfig;
            _groupsSelector = groupsSelector;
            _terrainGenerator = terrainGenerator;
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
                if (!EditorApplication.isPlaying)
                    return;
#endif
                try
                {
                    if (_stateModel.IsEnabled)
                    {
                        var context = _frameDataProvider.GetCurrentFrameData(_meshFilters);
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

        private async Task UpdateTerrain(IFrameData3b context, bool async, CancellationToken token)
        {
            context?.Activate();
            GroupActions<Vector3i> groupActions = null;
            IReadOnlyPooledDictionary<Vector3i, IPooledTerrainMesh> results = null;
            try
            {
                Func<IReadOnlyPooledDictionary<Vector3i, IPooledTerrainMesh>> computeAction = () =>
                {
                    using (_profiler.Start("Creating visible groups"))
                    {
                        groupActions = _groupsSelector.GetGroupsToUpdate(context);
                        if (groupActions.ToRecompute.Any())
                            _logger.LogMessage($"Groups to recompute {groupActions.ToRecompute.Count()}. Groups to keep {groupActions.ToKeep.Count()}.\r\nRecomputed:\r\n{groupActions.ToRecompute.StringJoin("\r\n")}");
                    }

                    var toRecompute = groupActions?.ToRecompute ?? Enumerable.Empty<Vector3i>();
                    if (toRecompute.Any())
                        using (_profiler.Start("TerrainMeshGenerator"))
                        {
                            PooledDictionary<Vector3i, IPooledTerrainMesh> meshes;
                            using (_profiler.Start("init"))
                            {
                                meshes = PooledDictionary<Vector3i, IPooledTerrainMesh>.Create();
                            }

                            foreach (var group in toRecompute)
                            {
                                var mesh = PooledTerrainMeshContainer<ExpandingUnityTerrainMesh>.CreateDirty();
                                _terrainGenerator.Generate(group, context.Image, mesh);
                                meshes[group] = mesh;
                            }

                            return meshes;
                        }

                    return null;
                };

                if (async)
                    results = await Task.Run(computeAction);
                else
                    results = computeAction();

                if (token.IsCancellationRequested)
                    return;

                if (results != null)
                    using (_profiler.Start("Applying mesh"))
                    {
                        var toKeepGroups = groupActions?.ToKeep ?? ReadOnlySet<Vector3i>.Empty;

                        if (results != null)
                            using (_profiler.Start("Setting Mesh"))
                            {
                                var toDeleteGroups = _meshFilters.Keys.Except(results.Keys)
                                    .Except(toKeepGroups)
                                    .ToList();

                                foreach (var terrainMesh in results)
                                {
                                    var group = terrainMesh.Key;
                                    var triangleMesh = terrainMesh.Value;

                                    if (terrainMesh.Value == null || triangleMesh.Mesh.VertexCount == 0)
                                    {
                                        if (_meshFilters.ContainsKey(group))
                                            _meshFilters[group]
                                                ?.Destroy();

                                        _meshFilters[group] = null;
                                        continue;
                                    }

                                    var unityData = _meshFilters.TryGetValue(group);
                                    if (unityData == null)
                                        unityData = _gameObjectFactory();


                                    triangleMesh.Mesh.SetUnityMesh(unityData);
                                    _meshFilters[group] = unityData;
                                }

                                foreach (var toDeleteGroup in toDeleteGroups)
                                {
                                    _meshFilters[toDeleteGroup]
                                        ?.Destroy();
                                    _meshFilters.Remove(toDeleteGroup);
                                }
                            }
                    }

                if (!async)
                    await UniTask.Yield();
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            finally
            {
                context?.Deactivate();
                groupActions?.Dispose();
                results?.Dispose();
            }
        }
    }
}