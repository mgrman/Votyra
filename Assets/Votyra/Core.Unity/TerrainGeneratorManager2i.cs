using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Votyra.Core.GroupSelectors;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core
{
    public class TerrainGeneratorManager2i : IDisposable, ITickable
    {
        private readonly Dictionary<Vector2i, ITerrainGroupGeneratorManager2i> _activeGroups =
            new Dictionary<Vector2i, ITerrainGroupGeneratorManager2i>();

        private readonly Vector2i _cellInGroupCount;

        private readonly IFrameDataProvider2i _frameDataProvider;

        private readonly HashSet<Vector2i> _groupsToRecompute = new HashSet<Vector2i>();

        private readonly IInterpolationConfig _interpolationConfig;

        private readonly int _meshTopologyDistance;

        private readonly CancellationTokenSource _onDestroyCts = new CancellationTokenSource();

        private readonly IStateModel _stateModel;

        private readonly ITerrainConfig _terrainConfig;

        private readonly TaskFactory _taskFactory = new TaskFactory();
        
        private readonly ITerrainGroupGeneratorManagerFactory2i _groupFactory;

        private bool _computedOnce;

        private Task _waitForTask = Task.CompletedTask;

        public TerrainGeneratorManager2i(ITerrainConfig terrainConfig, IStateModel stateModel, IFrameDataProvider2i frameDataProvider, IInterpolationConfig interpolationConfig,ITerrainGroupGeneratorManagerFactory2i groupFactory)
        {
            _terrainConfig = terrainConfig;
            _cellInGroupCount = _terrainConfig.CellInGroupCount.XY;
            _stateModel = stateModel;
            _frameDataProvider = frameDataProvider;
            _interpolationConfig = interpolationConfig;
            _meshTopologyDistance = _interpolationConfig.MeshSubdivision != 1 ? 2 : 1;
            _groupFactory = groupFactory;
        }

        public void Dispose()
        {
            _onDestroyCts.Cancel();
        }

        public void Tick()
        {
            if (_onDestroyCts.IsCancellationRequested || !_stateModel.IsEnabled || !_waitForTask.IsCompleted)
                return;

            var context = _frameDataProvider.GetCurrentFrameData(_meshTopologyDistance, _computedOnce);

            if (context != null)
            {
                context?.Activate();
                _waitForTask = _terrainConfig.Async
                    ? UpdateTerrainInBackground(context)
                    : UpdateTerrainInForegroud(context);
                _computedOnce = true;
            }
        }

        private Task UpdateTerrainInForegroud(IFrameData2i context)
        {
            UpdateTerrain(context);
            return Task.CompletedTask;
        }

        private Task UpdateTerrainInBackground(IFrameData2i context)
        {
            return _taskFactory.StartNew(UpdateTerrain, context);
        }

        private void UpdateTerrain(object context)
        {
            UpdateTerrain(context as IFrameData2i);
        }

        private void UpdateTerrain(IFrameData2i context)
        {
            HandleVisibilityUpdates(context);
            UpdateGroupManagers(context);
            context?.Deactivate();
        }

        private void UpdateGroupManagers(IFrameData2i context)
        {
            foreach (var activeGroup in _activeGroups)
            {
                activeGroup.Value.Update(context);
            }
        }

        private void HandleVisibilityUpdates(IFrameData2i context)
        {
            context.UpdateGroupsVisibility(_cellInGroupCount,
                _groupsToRecompute,
                OnAddedGroup,
                OnRemovedGroup);
        }

        private void OnRemovedGroup(Vector2i removedGroup)
        {
            _activeGroups.TryRemoveAndReturnValue(removedGroup)
                ?.Dispose();
        }

        private void OnAddedGroup(Vector2i newGroup)
        {
            _activeGroups.Add(newGroup, _groupFactory.CreateGroupManager(newGroup));
        }
    }
}