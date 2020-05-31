using System;
using UnityEngine;
using Votyra.Core;
using Votyra.Core.Behaviours;
using Votyra.Core.GroupSelectors;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Pooling;
using Votyra.Core.Queueing;
using Votyra.Core.Raycasting;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class Terrain2iInstaller : MonoInstaller
    {
        public void UsedOnlyForAOTCodeGeneration()
        {
            new TerrainGeneratorManager2i(null, null, null, null, null, null, null, null);

            // Include an exception so we can be sure to know if this method is ever called.
            throw new InvalidOperationException("This method is used for AOT code generation only. Do not call it at runtime.");
        }

        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<InterpolatedImage2iTo2fPostProcessor>()
                .AsSingle();

            this.Container.BindInterfacesAndSelfTo<InterpolatedUvPostProcessorStep>()
                .AsSingle()
                .When(c =>
                {
                    var config = c.Container.Resolve<IInterpolationConfig>();
                    return config.ImageSubdivision > 1;
                });

            this.Container.Bind<ScaleAdjustor>()
                .ToSelf()
                .AsSingle()
                .NonLazy();

            this.Container.BindInterfacesAndSelfTo<Terrain2fRaycaster>()
                .AsSingle()
                .NonLazy();

            this.Container.BindInterfacesAndSelfTo<TerrainGeneratorManager2i>()
                .AsSingle()
                .NonLazy();

            this.Container.BindInterfacesAndSelfTo<GroupsByCameraVisibilitySelector2i>()
                .AsSingle();

            this.Container.BindInterfacesAndSelfTo<BicubicTerrainMesher2f>()
                .AsSingle()
                .When(c =>
                {
                    var interpolationConfig = c.Container.Resolve<IInterpolationConfig>();
                    return (interpolationConfig.MeshSubdivision > 1) && (interpolationConfig.ActiveAlgorithm == IntepolationAlgorithm.Cubic);
                });

            this.Container.BindInterfacesAndSelfTo<TerrainMesher2f>()
                .AsSingle()
                .When(c =>
                {
                    var interpolationConfig = c.Container.Resolve<IInterpolationConfig>();
                    return (interpolationConfig.MeshSubdivision == 1) && (interpolationConfig.ImageSubdivision == 1);
                });

            this.Container.BindInterfacesAndSelfTo<FixedTerrainMeshPool>()
                .AsSingle()
                .When(c =>
                {
                    var interpolationConfig = c.Container.Resolve<IInterpolationConfig>();
                    return !interpolationConfig.DynamicMeshes;
                });

            this.Container.BindInterfacesAndSelfTo<ExpandingTerrainMeshPool>()
                .AsSingle()
                .When(c =>
                {
                    var interpolationConfig = c.Container.Resolve<IInterpolationConfig>();
                    return interpolationConfig.DynamicMeshes;
                });

            this.Container.BindInterfacesAndSelfTo<TerrainGameObjectPool>()
                .AsSingle();

            this.Container.BindInterfacesAndSelfTo<TerrainUnityMeshManager>()
                .AsSingle()
                .NonLazy();

            this.Container.BindInterfacesAndSelfTo<FrameData2iPool>()
                .AsSingle();

            this.Container.BindInterfacesAndSelfTo<FrameData2iProvider>()
                .AsSingle();

            this.Container.Bind<Func<GameObject>>()
                .FromMethod(context =>
                {
                    var root = context.Container.ResolveId<GameObject>("root");
                    var terrainConfig = context.Container.Resolve<ITerrainConfig>();
                    var materialConfig = context.Container.Resolve<IMaterialConfig>();
                    Func<GameObject> factory = () => this.CreateNewGameObject(root, terrainConfig, materialConfig);
                    return factory;
                })
                .AsSingle();

            this.Container.BindInterfacesAndSelfTo<PoolStats>()
                .FromNewComponentOn(this.gameObject)
                .AsSingle()
                .NonLazy();

            this.Container.BindInterfacesAndSelfTo<UnityTerrainGeneratorManager2i>()
                .AsSingle();

            this.Container.BindInterfacesAndSelfTo<TerrainRepository2i>()
                .AsSingle();

            this.Container.BindInterfacesAndSelfTo<LastValueTaskQueue<ArcResource<IFrameData2i>>>()
                .AsSingle()
                .When(c =>
                {
                    var terrainConfig = c.Container.Resolve<ITerrainConfig>();
                    return terrainConfig.AsyncTerrainGeneration;
                });

            this.Container.BindInterfacesAndSelfTo<PerGroupTaskQueue>()
                .AsSingle()
                .When(c =>
                {
                    var terrainConfig = c.Container.Resolve<ITerrainConfig>();
                    return terrainConfig.AsyncTerrainGeneration;
                });

            this.Container.BindInterfacesAndSelfTo<ImmediateQueue<ArcResource<IFrameData2i>>>()
                .AsSingle()
                .When(c =>
                {
                    var terrainConfig = c.Container.Resolve<ITerrainConfig>();
                    return !terrainConfig.AsyncTerrainGeneration;
                });

            this.Container.BindInterfacesAndSelfTo<ImmediateQueue<GroupUpdateData>>()
                .AsSingle()
                .When(c =>
                {
                    var terrainConfig = c.Container.Resolve<ITerrainConfig>();
                    return !terrainConfig.AsyncTerrainGeneration;
                });
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

            var name = string.Format("group_{0}", Guid.NewGuid());
            go.name = name;
            go.hideFlags = HideFlags.DontSave;

            var meshFilter = go.GetOrAddComponent<MeshFilter>();
            go.AddComponentIfMissing<MeshRenderer>();
            switch (terrainConfig.ColliderType)
            {
                case ColliderType.None:
                    break;
                case ColliderType.Box:
                    go.AddComponentIfMissing<BoxCollider>();
                    break;
                case ColliderType.Mesh:
                    go.AddComponentIfMissing<MeshCollider>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (meshFilter.sharedMesh == null)
            {
                meshFilter.mesh = new Mesh();
            }

            var mesh = meshFilter.sharedMesh;
            mesh.MarkDynamic();

            return go;
        }

        private class ScaleAdjustor
        {
            public ScaleAdjustor(IInterpolationConfig interpolationConfig, [Inject(Id = "root")]GameObject root)
            {
                var scale = 1f / interpolationConfig.ImageSubdivision;

                root.transform.localScale = new Vector3(root.transform.localScale.x * scale, root.transform.localScale.y * scale, root.transform.localScale.z);
            }
        }
    }
}
