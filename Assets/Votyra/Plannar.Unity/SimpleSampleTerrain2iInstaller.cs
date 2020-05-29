using Votyra.Core.Images.Constraints;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class SimpleSampleTerrain2IInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Rebind<IImageConstraint2i>()
                .To<SimpleTycoonTileConstraint2i>()
                .AsSingle();
        }
    }
}
