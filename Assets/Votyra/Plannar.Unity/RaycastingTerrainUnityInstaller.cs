using System;
using UnityEngine;
using Votyra.Core;
using Votyra.Core.Behaviours;
using Votyra.Core.GroupSelectors;
using Votyra.Core.Images;
using Votyra.Core.Images.Constraints;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Painting;
using Votyra.Core.Painting.Commands;
using Votyra.Core.Pooling;
using Votyra.Core.Queueing;
using Votyra.Core.Raycasting;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class RaycastingTerrainUnityInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {

            Container.BindInterfacesAndSelfTo<SingleRaycaster>()
                .AsSingle()
                .NonLazy();
        }
    }
}