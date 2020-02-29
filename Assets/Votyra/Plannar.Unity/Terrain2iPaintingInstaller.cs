using UnityEngine;
using Votyra.Core.Painting;
using Votyra.Core.Painting.Commands;
using Votyra.Core.Unity.Painting;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class Terrain2iPaintingInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UnityInputManager>()
                .FromNewComponentOn(c => c.Container.ResolveId<GameObject>("root"))
                .AsSingle()
                .NonLazy();
            
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