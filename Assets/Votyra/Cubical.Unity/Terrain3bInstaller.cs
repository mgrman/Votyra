using System;
using UnityEngine;
using Votyra.Core;
using Votyra.Core.Behaviours;
using Votyra.Core.GroupSelectors;
using Votyra.Core.Images;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.TerrainGenerators;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Cubical.Unity
{
    public class Terrain3bInstaller : MonoInstaller
    {
        public void UsedOnlyForAOTCodeGeneration()
        {
            new TerrainGeneratorManager3b(null, null, null, null, null, null, null, null);

            // Include an exception so we can be sure to know if this method is ever called.
            throw new InvalidOperationException("This method is used for AOT code generation only. Do not call it at runtime.");
        }

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ImageConfig>().AsSingle();
            Container.BindInterfacesAndSelfTo<InitialImageConfig>().AsSingle();
            Container.BindInterfacesAndSelfTo<TerrainConfig>().AsSingle();
            Container.BindInterfacesAndSelfTo<MaterialConfig>().AsSingle();

            Container.BindInterfacesAndSelfTo<TerrainGenerator3b>().AsSingle();
            Container.BindInterfacesAndSelfTo<GroupsByCameraVisibilitySelector3B>().AsSingle();
            Container.BindInterfacesAndSelfTo<InitialStateSetter3b>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<EditableMatrixImage3b>().AsSingle();
            Container.BindInstance<GameObject>(this.gameObject).WithId("root").AsSingle();
            Container.BindInterfacesAndSelfTo<ClickToPaint3b>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<TerrainGeneratorManager3b>().AsSingle().NonLazy();
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

            string name = string.Format("group_{0}", Guid.NewGuid());
            go.name = name;
            go.hideFlags = HideFlags.DontSave;

            var meshFilter = go.GetOrAddComponent<MeshFilter>();
            go.AddComponentIfMissing<MeshRenderer>();
            go.AddComponentIfMissing<MeshCollider>();

            if (meshFilter.sharedMesh == null)
            {
                meshFilter.mesh = new Mesh();
            }

            var mesh = meshFilter.sharedMesh;
            mesh.MarkDynamic();

            return go;
        }
    }
}