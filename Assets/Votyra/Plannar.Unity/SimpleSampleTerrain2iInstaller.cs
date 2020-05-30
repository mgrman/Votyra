using Votyra.Core.Images.Constraints;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class SimpleSampleTerrain2IInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Rebind<IImageConstraint2I>()
                .To<SimpleTycoonTileConstraint2I>()
                .AsSingle();
        }
    }
}
