using System;
using UnityEngine;
using Votyra.Core;
using Votyra.Core.Behaviours;
using Votyra.Core.GroupSelectors;
using Votyra.Core.Images;
using Votyra.Core.Images.Constraints;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Painting;
using Votyra.Core.Painting.Commands;
using Votyra.Core.Pooling;
using Votyra.Core.Raycasting;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Unity.Painting;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class Terrain2iInstaller : MonoInstaller
    {
        public void UsedOnlyForAOTCodeGeneration()
        {
            new TerrainGeneratorManager2i(null, null, null, null,null,null);

            // Include an exception so we can be sure to know if this method is ever called.
            throw new InvalidOperationException("This method is used for AOT code generation only. Do not call it at runtime.");
        }

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ImageConfig>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<InitialImageConfig>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<TerrainConfig>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<MaterialConfig>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<InterpolationConfig>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<ConstraintConfig>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<InitialStateSetter2f>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<EditableMatrixImage2f>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<EditableMatrixMask2e>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<InterpolatedImage2iTo2fPostProcessor>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<InterpolatedUVPostProcessorStep>()
                .AsSingle()
                .When(c =>
                {
                    var config = c.Container.Resolve<IInterpolationConfig>();
                    return config.ImageSubdivision > 1;
                });
            Container.Bind<ScaleAdjustor>()
                .ToSelf()
                .AsSingle()
                .NonLazy();

            var meshRoot = new GameObject("MeshRoot");
            meshRoot.transform.SetParent(transform, false);
            Container.BindInstance(meshRoot)
                .WithId("root")
                .AsSingle();

            Container.BindInterfacesAndSelfTo<Terrain2fRaycaster>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<PaintingSelectionManager>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<PaintingModel>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<Flatten>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<IncreaseOrDecrease>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<MakeOrRemoveHole>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<PaintingGui>()
                .FromNewComponentOn(gameObject)
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<TerrainGeneratorManager2i>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<GroupsByCameraVisibilitySelector2i>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<BicubicTerrainMesher2f>()
                .AsSingle()
                .When(c =>
                {
                    var interpolationConfig = c.Container.Resolve<IInterpolationConfig>();
                    return interpolationConfig.MeshSubdivision > 1 && interpolationConfig.ActiveAlgorithm == IntepolationAlgorithm.Cubic;
                });

            Container.BindInterfacesAndSelfTo<TerrainMesher2f>()
                .AsSingle()
                .When(c =>
                {
                    var interpolationConfig = c.Container.Resolve<IInterpolationConfig>();
                    return interpolationConfig.MeshSubdivision == 1 && interpolationConfig.ImageSubdivision == 1;
                });

            Container.BindInterfacesAndSelfTo<FixedTerrainMeshPool>()
                .AsSingle()
                .When(c =>
                {
                    var interpolationConfig = c.Container.Resolve<IInterpolationConfig>();
                    return !interpolationConfig.DynamicMeshes;
                });

            Container.BindInterfacesAndSelfTo<ExpandingTerrainMeshPool>()
                .AsSingle()
                .When(c =>
                {
                    var interpolationConfig = c.Container.Resolve<IInterpolationConfig>();
                    return interpolationConfig.DynamicMeshes;
                });

            Container.BindInterfacesAndSelfTo<TerrainGameObjectPool>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<TerrainUnityMeshManager>()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesAndSelfTo<FrameData2iPool>()
                .AsSingle();
            
            Container.BindInterfacesAndSelfTo<FrameData2iProvider>()
                .AsSingle();

            Container.Bind<Func<GameObject>>()
                .FromMethod(context =>
                {
                    var root = context.Container.ResolveId<GameObject>("root");
                    var terrainConfig = context.Container.Resolve<ITerrainConfig>();
                    var materialConfig = context.Container.Resolve<IMaterialConfig>();
                    Func<GameObject> factory = () => CreateNewGameObject(root, terrainConfig, materialConfig);
                    return factory;
                })
                .AsSingle();

            Container.BindInterfacesAndSelfTo<PoolStats>()
                .FromNewComponentOn(this.gameObject)
                .AsSingle()
                .NonLazy();
            
            Container.BindInterfacesAndSelfTo<PopulatorConfig>()
                .AsSingle();
            
            Container.BindInterfacesAndSelfTo<TerrainPopulatorManager>()
                .FromNewComponentOn(this.gameObject)
                .AsSingle()
                .NonLazy();
            
            Container.BindInterfacesAndSelfTo<UnityTerrainGeneratorManager2i>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<TerrainRepository2i>()
                .AsSingle();
        }

        private GameObject CreateNewGameObject(GameObject root, ITerrainConfig terrainConfig, IMaterialConfig materialConfig)
        {
            var go = new GameObject();
            go.transform.SetParent(root.transform, false);
            if (terrainConfig.DrawBounds)
                go.AddComponent<DrawBounds>();
            var meshRenderer = go.GetOrAddComponent<MeshRenderer>();
            meshRenderer.materials = ArrayUtils.CreateNonNull(materialConfig.Material, materialConfig.MaterialWalls);

            var name = string.Format("group_{0}", Guid.NewGuid());
            go.name = name;
            go.hideFlags = HideFlags.DontSave;

            var meshFilter = go.GetOrAddComponent<MeshFilter>();
            go.AddComponentIfMissing<MeshRenderer>();
            if (terrainConfig.UseMeshCollider)
                go.AddComponentIfMissing<MeshCollider>();
            else
                go.AddComponentIfMissing<BoxCollider>();

            if (meshFilter.sharedMesh == null)
                meshFilter.mesh = new Mesh();

            var mesh = meshFilter.sharedMesh;
            mesh.MarkDynamic();

            return go;
        }

        private class ScaleAdjustor
        {
            public ScaleAdjustor(IInterpolationConfig interpolationConfig, [Inject(Id = "root")] GameObject root)
            {
                var scale = 1f / interpolationConfig.ImageSubdivision;

                root.transform.localScale = new Vector3(root.transform.localScale.x * scale, root.transform.localScale.y * scale, root.transform.localScale.z);
            }
        }
    }
}