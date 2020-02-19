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
    public class Terrain2iPaintingInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PaintingModel>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<IncreaseFactory>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<IncreaseLargeFactory>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<DecreaseFactory>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<DecreaseLargeFactory>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<FlattenFactory>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<FlattenLargeFactory>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<MakeHoleFactory>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<MakeHoleLargeFactory>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<RemoveHoleFactory>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<RemoveHoleLargeFactory>()
                .AsSingle()
                .NonLazy();
        }
    }
}