using Votyra.Core;
using Votyra.Core.Images.Constraints;
using Votyra.Core.ImageSamplers;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.Unity.Config;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class DualSampleTerrain2iInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Rebind<IImageConstraint2i>()
                .To<DualSampledTycoonTileConstraint2i>()
                .AsSingle();
            Container.Rebind<ITerrainVertexPostProcessor>()
                .To<WallsVertexPostProcessor>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<DualSampleTerrainUVPostProcessor>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<DualSampleConfig>()
                .AsSingle();
        }
    }
}