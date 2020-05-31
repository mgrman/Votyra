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
            this.Container.BindInterfacesAndSelfTo<UnityInputManager>()
                .FromNewComponentOn(c => c.Container.ResolveId<GameObject>("root"))
                .AsSingle()
                .NonLazy();

            this.Container.BindInterfacesAndSelfTo<PaintingModel>()
                .AsSingle()
                .NonLazy();

            this.Container.BindInterfacesAndSelfTo<IncreaseFactory>()
                .AsSingle()
                .NonLazy();

            this.Container.BindInterfacesAndSelfTo<IncreaseLargeFactory>()
                .AsSingle()
                .NonLazy();

            this.Container.BindInterfacesAndSelfTo<DecreaseFactory>()
                .AsSingle()
                .NonLazy();

            this.Container.BindInterfacesAndSelfTo<DecreaseLargeFactory>()
                .AsSingle()
                .NonLazy();

            this.Container.BindInterfacesAndSelfTo<FlattenFactory>()
                .AsSingle()
                .NonLazy();

            this.Container.BindInterfacesAndSelfTo<FlattenLargeFactory>()
                .AsSingle()
                .NonLazy();

            // Container.BindInterfacesAndSelfTo<MakeHoleFactory>()
            //         .AsSingle()
            //         .NonLazy();
            //     Container.BindInterfacesAndSelfTo<MakeHoleLargeFactory>()
            //         .AsSingle()
            //         .NonLazy();
            //     Container.BindInterfacesAndSelfTo<RemoveHoleFactory>()
            //         .AsSingle()
            //         .NonLazy();
            //     Container.BindInterfacesAndSelfTo<RemoveHoleLargeFactory>()
            //         .AsSingle()
            //         .NonLazy();
        }
    }
}
