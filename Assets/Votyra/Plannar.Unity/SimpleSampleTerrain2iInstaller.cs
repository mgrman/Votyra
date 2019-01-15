using Votyra.Core.Images.Constraints;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class SimpleSampleTerrain2iInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Rebind<IImageConstraint2i>()
                .To<SimpleTycoonTileConstraint2i>()
                .AsSingle();
        }
    }
}