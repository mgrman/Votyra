using Votyra.Core.Images;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class ImageConfigInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<ImageConfig>()
                .AsSingle();
        }
    }
}
