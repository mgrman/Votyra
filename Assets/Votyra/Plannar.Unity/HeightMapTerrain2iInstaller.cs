using Votyra.Core.Images.Constraints;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class HeightMapTerrain2iInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Unbind<IImageConstraint2i>();
        }
    }
}
