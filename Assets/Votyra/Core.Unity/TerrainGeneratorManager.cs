using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.GroupSelectors;
using Votyra.Core.TerrainGenerators;
using Votyra.Core;
using Votyra.Core.Logging;

namespace Votyra.Core
{
    //TODO: move to floats
    public class TerrainGeneratorManager<TFrameData, TGroupKey> : IDisposable
        where TFrameData : IFrameData
    {
        protected readonly IThreadSafeLogger _logger;

        protected readonly ITerrainConfig _terrainConfig;

        protected readonly IGroupSelector<TFrameData, TGroupKey> _groupsSelector;

        protected readonly ITerrainGenerator<TFrameData, TGroupKey> _terrainGenerator;

        protected readonly IMeshUpdater<TGroupKey> _meshUpdater;

        protected readonly IStateModel _stateModel;

        protected readonly IProfiler _profiler;

        protected readonly IFrameDataProvider<TFrameData> _frameDataProvider;

        private CancellationTokenSource _onDestroyCts = new CancellationTokenSource();

        public TerrainGeneratorManager(IThreadSafeLogger logger, ITerrainConfig terrainConfig, IGroupSelector<TFrameData, TGroupKey> groupsSelector, ITerrainGenerator<TFrameData, TGroupKey> terrainGenerator, IMeshUpdater<TGroupKey> meshUpdater, IStateModel stateModel, IProfiler profiler, IFrameDataProvider<TFrameData> frameDataProvider)
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

        private async void StartUpdateing()
        {
            while (!_onDestroyCts.IsCancellationRequested)
            {
                try
                {
                    if (_stateModel.IsEnabled)
                    {
                        var context = _frameDataProvider.GetCurrentFrameData();
                        await UpdateTerrain(context, _terrainConfig.Async, _onDestroyCts.Token);
                    }
                    else
                    {
                        await Task.Delay(100);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex);
                }
            }
        }

        private async Task UpdateTerrain(TFrameData context, bool async, CancellationToken token)
        {
            GroupActions<TGroupKey> groupActions = null;
            IReadOnlyPooledDictionary<TGroupKey, ITerrainMesh> results = null;
            try
            {
                Func<IReadOnlyPooledDictionary<TGroupKey, ITerrainMesh>> computeAction = () =>
                {
                    using (_profiler.Start("Creating visible groups"))
                    {
                        groupActions = _groupsSelector.GetGroupsToUpdate(context);
                        if (groupActions.ToRecompute.Any())
                        {
                            _logger.LogMessage($"Groups to recompute {groupActions.ToRecompute.Count()}. Groups to keep {groupActions.ToKeep.Count()}.");
                        }
                    }
                    var toRecompute = groupActions?.ToRecompute ?? Enumerable.Empty<TGroupKey>();
                    if (toRecompute.Any())
                    {
                        using (_profiler.Start("TerrainMeshGenerator"))
                        {
                            return _terrainGenerator.Generate(context, toRecompute);
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
                        var toKeep = groupActions?.ToKeep ?? ReadOnlySet<TGroupKey>.Empty;
                        _meshUpdater.UpdateMesh(results, toKeep);
                    }
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

        public void Dispose()
        {
            _onDestroyCts.Cancel();
        }
    }
}