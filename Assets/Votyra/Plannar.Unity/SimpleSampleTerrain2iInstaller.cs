using Votyra.Core.Images;
using Votyra.Core.Images.Constraints;
using Votyra.Core.ImageSamplers;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Plannar.Images.Constraints;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class SimpleSampleTerrain2iInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Rebind<IImageConstraint2i>().To<SimpleTycoonTileConstraint2i>().AsSingle();
            Container.Rebind<ITerrainMesher2f>().To<BicubicTerrainMesher2i>().AsSingle();
        }
    }
}