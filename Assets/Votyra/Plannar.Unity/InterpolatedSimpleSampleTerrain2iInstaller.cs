using UnityEngine;
using Votyra.Core;
using Votyra.Core.Images;
using Votyra.Core.Images.Constraints;
using Votyra.Core.ImageSamplers;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Plannar.Images.Constraints;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class InterpolatedSimpleSampleTerrain2iInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Rebind<IImageConstraint2i>().To<SimpleTycoonTileConstraint2i>().AsSingle();
            Container.Rebind<ITerrainMesher2f>().To<DynamicTerrainMesher2f>().AsSingle();
            Container.Rebind<IImage2fProvider>().To<InterpolatedImage2iTo2fProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<InterpolatedUVPostProcessorStep>().AsSingle();
            Container.Bind<ScaleAdjustor>().ToSelf().AsSingle().NonLazy();
        }

        private class ScaleAdjustor
        {

            public  ScaleAdjustor(IInterpolationConfig interpolationConfig,[Inject(Id = "root")]GameObject root)
            {
                var scale = 1f / interpolationConfig.Subdivision;
                
                root.transform.localScale=new Vector3(root.transform.localScale.x*scale, root.transform.localScale.y*scale,
                    root.transform.localScale.z);
            }
        }
    }
}