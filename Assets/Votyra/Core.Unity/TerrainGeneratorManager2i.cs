using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Votyra.Core.GroupSelectors;
using Votyra.Core.Models;
using Votyra.Core.Unity.TerraingGroupGenerator;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Unity
{
    public class TerrainGeneratorManager2i : IDisposable, ITickable
    {
        private readonly Dictionary<Vector2i, ITerrainGroupGeneratorManager2i> _activeGroups =
            new Dictionary<Vector2i, ITerrainGroupGeneratorManager2i>();

        private readonly IFrameDataProvider2i _frameDataProvider;

        private readonly CancellationTokenSource _onDestroyCts = new CancellationTokenSource();

        private readonly IStateModel _stateModel;

        private readonly TaskFactory _taskFactory = new TaskFactory();
        
        private readonly ITerrainGroupGeneratorManagerFactory2i _groupFactory;
        
        private readonly IGroupsByCameraVisibilitySelector2i _groupsByCameraVisibilitySelector;

        private readonly Func<IFrameData2i, Task> _updateFunc; 

        private bool _computedOnce;

        private Task _waitForTask = Task.CompletedTask;

        public TerrainGeneratorManager2i(ITerrainConfig terrainConfig, IStateModel stateModel, IFrameDataProvider2i frameDataProvider, IInterpolationConfig interpolationConfig,ITerrainGroupGeneratorManagerFactory2i groupFactory, IGroupsByCameraVisibilitySelector2i groupsByCameraVisibilitySelector)
        {
            _stateModel = stateModel;
            _frameDataProvider = frameDataProvider;
            _groupFactory = groupFactory;
            _groupsByCameraVisibilitySelector = groupsByCameraVisibilitySelector;
            _groupsByCameraVisibilitySelector.OnAdd += OnAddedGroup;
            _groupsByCameraVisibilitySelector.OnRemove += OnRemovedGroup;
            _updateFunc = terrainConfig.Async
                ? (Func<IFrameData2i, Task>)UpdateTerrainInBackground
                : (Func<IFrameData2i, Task>)UpdateTerrainInForegroud;
        }

        public void Dispose()
        {
            _onDestroyCts.Cancel();
        }

        public void Tick()
        {
            if (_onDestroyCts.IsCancellationRequested || !_stateModel.IsEnabled || !_waitForTask.IsCompleted)
                return;

            var context = _frameDataProvider.GetCurrentFrameData(_computedOnce);

            if (context != null)
            {
                context?.Activate();
                _waitForTask = _updateFunc(context);
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
            _groupsByCameraVisibilitySelector.UpdateGroupsVisibility(context);
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