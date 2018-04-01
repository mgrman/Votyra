using Votyra.Core.Images.Constraints;
using Votyra.Core.ImageSamplers;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Plannar.Images.Constraints;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class DualSampleTerrain2iInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Rebind<IImageSampler2i>().To<DualImageSampler2i>().AsSingle();
            Container.Rebind<IImageConstraint2i>().To<DualSampledTycoonTileConstraint2i>().AsSingle();
            Container.Rebind<ITerrainMesher2i>().To<TerrainMesherWithWalls2i>().AsSingle();
        }
    }
}