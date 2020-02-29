using Votyra.Core.Images;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class LayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<LayerConfig>()
                .AsSingle();
        }
    }
}