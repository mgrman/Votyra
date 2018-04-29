using Votyra.Core.ImageSamplers;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Zenject;

namespace Votyra.Cubical.Unity
{
    public class ConstrainedDualSampleTerrain3bInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Rebind<IImageSampler3>().To<ConstrainedDualImageSampler3b>().AsSingle();
            // Container.Rebind<IImageConstraint3i>().To<BooleanConstraint3i>().AsSingle();
            Container.Rebind<ITerrainMesher3b>().To<TerrainMesherWithConstrainedWalls3b>().AsSingle();
        }
    }
}