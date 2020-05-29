using Votyra.Core.Images;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class LayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<LayerConfig>()
                .AsSingle();
        }
    }
}
