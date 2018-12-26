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
            Container.Unbind<IImageConstraint2i>();
            Container.Rebind<ITerrainMesher2f>().To<BicubicTerrainMesher2i>().AsSingle();
        }
    }
}