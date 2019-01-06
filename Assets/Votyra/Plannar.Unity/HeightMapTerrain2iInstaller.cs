using Votyra.Core.Images.Constraints;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class HeightMapTerrain2iInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Unbind<IImageConstraint2i>();
        }
    }
}