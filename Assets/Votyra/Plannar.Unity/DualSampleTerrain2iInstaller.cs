using Votyra.Core;
using Votyra.Core.Images.Constraints;
using Votyra.Core.ImageSamplers;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class DualSampleTerrain2IInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Rebind<IImageConstraint2i>()
                .To<DualSampledTycoonTileConstraint2i>()
                .AsSingle();
            this.Container.Rebind<ITerrainVertexPostProcessor>()
                .To<WallsVertexPostProcessor>()
                .AsSingle();
            this.Container.BindInterfacesAndSelfTo<DualSampleTerrainUVPostProcessor>()
                .AsSingle();
            this.Container.BindInterfacesAndSelfTo<DualSampleConfig>()
                .AsSingle();
        }
    }
}
