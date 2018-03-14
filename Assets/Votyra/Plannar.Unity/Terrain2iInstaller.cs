using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Votyra.Plannar.GroupSelectors;
using Votyra.Plannar.Images;
using Votyra.Plannar.Images.Constraints;
using Votyra.Plannar.ImageSamplers;
using Votyra.Plannar.TerrainGenerators;
using Votyra.Plannar.TerrainGenerators.TerrainMeshers;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class Terrain2iInstaller : MonoInstaller
    {

        public override void InstallBindings()
        {
            Container.Bind<ITerrainGenerator2i>().To<TerrainGenerator2i>().AsSingle();
            Container.Bind<ITerrainMesher2i>().To<ColumnTerrainMesher2i>().AsSingle();
            Container.Bind<IMeshUpdater<Vector2i>>().To<TerrainMeshUpdater<Vector2i>>().AsSingle();
            Container.Bind<IGroupSelector2i>().To<GroupsByCameraVisibilitySelector2i>().AsSingle();

            Container.BindInterfacesAndSelfTo<EditableMatrixImage2f>().AsSingle();
            Container.BindInterfacesAndSelfTo<EditableImage2fInitialStateSetter>().AsSingle().NonLazy();

            var root = new GameObject("terrain");
            root.transform.SetParent(this.transform, false);
            Container.BindInstance<GameObject>(root).WithId("root").AsSingle();

            Container.BindInterfacesAndSelfTo<ClickToPaint>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<TerrainGeneratorBehaviour2i>().AsSingle().NonLazy();
        }
    }
}