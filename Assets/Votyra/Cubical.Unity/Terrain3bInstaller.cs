using UnityEngine;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Votyra.Core.GroupSelectors;
using Votyra.Core.ImageSamplers;
using Votyra.Core.TerrainGenerators;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Zenject;
using Votyra.Core.Images.Constraints;

namespace Votyra.Cubical.Unity
{
    public class Terrain3bInstaller : MonoInstaller
    {

        public override void InstallBindings()
        {
            Container.Bind<ITerrainGenerator3b>().To<TerrainGenerator3b>().AsSingle();
            Container.Bind<IMeshUpdater<Vector3i>>().To<TerrainMeshUpdater<Vector3i>>().AsSingle();
            Container.Bind<IGroupSelector3i>().To<GroupsByCameraVisibilitySelector3i>().AsSingle();

            Container.BindInterfacesAndSelfTo<UmbraImageProvider3f>().AsSingle();
            // Container.BindInterfacesAndSelfTo<EditableImage3fInitialStateSetter>().AsSingle().NonLazy();

            Container.BindInstance<GameObject>(this.gameObject).WithId("root").AsSingle();

            // Container.BindInterfacesAndSelfTo<ClickToPaint>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<TerrainGeneratorManager3b>().AsSingle().NonLazy();
        }

    }
}