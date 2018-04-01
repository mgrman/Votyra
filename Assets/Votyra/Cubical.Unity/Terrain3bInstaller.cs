using UnityEngine;
using Votyra.Core;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Votyra.Core.GroupSelectors;
using Votyra.Core.ImageSamplers;
using Votyra.Core.TerrainGenerators;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Zenject;
using Votyra.Core.Images.Constraints;
using Votyra.Core.Images;
using Votyra.Core.Behaviours;
using System;

namespace Votyra.Cubical.Unity
{
    public class Terrain3bInstaller : MonoInstaller
    {

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TerrainGenerator3b>().AsSingle();
            Container.BindInterfacesAndSelfTo<TerrainMeshUpdater<Vector3i>>().AsSingle();
            Container.BindInterfacesAndSelfTo<GroupsByCameraVisibilitySelector3i>().AsSingle();
            Container.BindInterfacesAndSelfTo<InitialStateSetter3b>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<EditableMatrixImage3b>().AsSingle();
            Container.BindInstance<GameObject>(this.gameObject).WithId("root").AsSingle();
            Container.BindInterfacesAndSelfTo<ClickToPaint3b>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<TerrainGeneratorManager<IFrameData3b, Vector3i>>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<FrameData3bProvider>().AsSingle();

            Container.Bind<Func<GameObject>>()
                .FromMethod(context =>
                {
                    var root = context.Container.ResolveId<GameObject>("root");
                    var terrainConfig = context.Container.Resolve<ITerrainConfig>();
                    var materialConfig = context.Container.Resolve<IMaterialConfig>();
                    Func<GameObject> factory = () => CreateNewGameObject(root, terrainConfig, materialConfig);
                    return factory;

                }).AsSingle();
        }

        private GameObject CreateNewGameObject(GameObject root, ITerrainConfig terrainConfig, IMaterialConfig materialConfig)
        {
            var go = new GameObject();
            go.transform.SetParent(root.transform, false);
            if (terrainConfig.DrawBounds)
            {
                go.AddComponent<DrawBounds>();
            }
            var meshRenderer = go.GetOrAddComponent<MeshRenderer>();
            meshRenderer.materials = ArrayUtils.CreateNonNull(materialConfig.Material, materialConfig.MaterialWalls);
            return go;
        }

    }
}