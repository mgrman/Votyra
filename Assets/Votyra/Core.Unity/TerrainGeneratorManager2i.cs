using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using Votyra.Core.GroupSelectors;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Logging;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Profiling;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core
{
    //TODO: move to floats
    public class TerrainGeneratorManager2i : IDisposable
    {
        public readonly Dictionary<Vector2i, ITerrainGroupGeneratorManager2i> _activeGroups = new Dictionary<Vector2i, ITerrainGroupGeneratorManager2i>();

        private readonly Vector2i _cellInGroupCount;
        private readonly IFrameDataProvider2i _frameDataProvider;
        private readonly int _meshTopologyDistance;

        private readonly CancellationTokenSource _onDestroyCts = new CancellationTokenSource();
        private readonly TaskFactory _taskFactory = new TaskFactory();
        private readonly ITerrainConfig _terrainConfig;

        private readonly IGroupsByCameraVisibilitySelector2i _groupsByCameraVisibilitySelector2I;

        private Task _waitForTask = Task.CompletedTask;

        public TerrainGeneratorManager2i(ITerrainConfig terrainConfig, IFrameDataProvider2i frameDataProvider, ITerrainGroupGeneratorManagerPool managerPool, IGroupsByCameraVisibilitySelector2i groupsByCameraVisibilitySelector2I)
        {
            _terrainConfig = terrainConfig;
            _cellInGroupCount = _terrainConfig.CellInGroupCount.XY();
            _frameDataProvider = frameDataProvider;
            _groupsByCameraVisibilitySelector2I = groupsByCameraVisibilitySelector2I;

            create = (g) =>
            {
                var manager = managerPool.GetRaw();
                manager.Group = g;
                return manager;
            };

            dispose = (manager) =>
            {
                manager.Stop();
                managerPool.ReturnRaw(manager);
            };

            if (_terrainConfig.Async)
                _frameDataProvider.FrameData += UpdateTerrainInBackground;
            else
                _frameDataProvider.FrameData += UpdateTerrainInForeground;
        }

        public void Dispose()
        {
            _onDestroyCts.Cancel();

            if (_terrainConfig.Async)
                _frameDataProvider.FrameData -= UpdateTerrainInBackground;
            else
                _frameDataProvider.FrameData -= UpdateTerrainInForeground;

            foreach (var pair in _activeGroups)
            {
                pair.Value.Stop();
            }
        }

        public ITerrainMesh GetMeshForGroup(Vector2i group) =>
            _activeGroups.TryGetValue(group)
                ?.Mesh;

        private void UpdateTerrainInForeground(ArcResource<IFrameData2i> context)
        {
            UpdateTerrain(context);
        }

        private void UpdateTerrainInBackground(ArcResource<IFrameData2i> context)
        {
            if (!_waitForTask.IsCompleted)
            {
                context.Dispose();
                return;
            }

            _waitForTask = _taskFactory.StartNew(UpdateTerrain, context);
        }

        private void UpdateTerrain(object context)
        {
            UpdateTerrain(context as ArcResource<IFrameData2i>);
        }

        private void UpdateTerrain(ArcResource<IFrameData2i> context)
        {
            HandleVisibilityUpdates(context.Value);
            foreach (var activeGroup in _activeGroups)
            {
                activeGroup.Value.Update(context.Activate());
            }

            context.Dispose();
            context = null;
        }

        private void HandleVisibilityUpdates(IFrameData2i context)
        {
            _groupsByCameraVisibilitySelector2I.UpdateGroupsVisibility(context, _cellInGroupCount, _activeGroups, create, dispose);
        }

        private Func<Vector2i, ITerrainGroupGeneratorManager2i> create;
        private Action<ITerrainGroupGeneratorManager2i> dispose;
    }
}