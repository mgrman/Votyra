using System;
using UnityEngine;
using Votyra.Core;
using Votyra.Core.Behaviours;
using Votyra.Core.GroupSelectors;
using Votyra.Core.Images;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Painting;
using Votyra.Core.Painting.Commands;
using Votyra.Core.Painting.UI;
using Votyra.Core.TerrainGenerators;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class Terrain2iInstaller : MonoInstaller
    {
        public void UsedOnlyForAOTCodeGeneration()
        {
            new TerrainMeshUpdater<Vector2i>(null, null);
            new TerrainGeneratorManager<IFrameData2i, Vector2i>(null, null, null, null, null, null, null, null);

            // Include an exception so we can be sure to know if this method is ever called.
            throw new InvalidOperationException("This method is used for AOT code generation only. Do not call it at runtime.");
        }

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ImageConfig>().AsSingle();
            Container.BindInterfacesAndSelfTo<InitialImageConfig>().AsSingle();
            Container.BindInterfacesAndSelfTo<TerrainConfig>().AsSingle();
            Container.BindInterfacesAndSelfTo<MaterialConfig>().AsSingle();

            Container.BindInterfacesAndSelfTo<TerrainGenerator2i>().AsSingle();
            Container.BindInterfacesAndSelfTo<TerrainMeshUpdater<Vector2i>>().AsSingle();
            Container.BindInterfacesAndSelfTo<GroupsByCameraVisibilitySelector2i>().AsSingle();
            Container.BindInterfacesAndSelfTo<InitialStateSetter2f>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<EditableMatrixImage2i>().AsSingle();
            Container.BindInterfacesAndSelfTo<EditableMatrixMask2e>().AsSingle();
            Container.BindInstance<GameObject>(this.gameObject).WithId("root").AsSingle();

            Container.BindInterfacesAndSelfTo<PaintingInvocationManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PaintingSelectionManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<PaintingModel>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<Flatten>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<IncreaseOrDecrease>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MakeOrRemoveHole>().AsSingle().NonLazy();
            Container.Bind<Canvas>().FromComponentInNewPrefabResource("PaintingUI").AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<TerrainGeneratorManager<IFrameData2i, Vector2i>>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<FrameData2iProvider>().AsSingle();

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