using Votyra.Core.Images;
using Votyra.Core.Raycasting;
using Zenject;

namespace Votyra.Core.Unity
{
    public class ImageAggregatorInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<ImageAggregator>()
                .AsSingle();

            this.Container.BindInterfacesAndSelfTo<AggregatorRaycaster>()
                .AsSingle();
        }
    }
}
