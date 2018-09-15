using Votyra.Core.Images.Constraints;
using Votyra.Core.ImageSamplers;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class HeightMapTerrain2iInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Rebind<IImageSampler2i>().To<SimpleImageSampler2i>().AsSingle();
            Container.Unbind<IImageConstraint2i>();
            Container.Rebind<ITerrainMesher2i>().To<BicubicTerrainMesher2i>().AsSingle();
        }
    }
}