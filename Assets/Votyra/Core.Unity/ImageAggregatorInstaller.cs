using Votyra.Core.Images;
using Votyra.Core.Raycasting;
using Zenject;

namespace Votyra.Core.Unity
{
    public class ImageAggregatorInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ImageAggregator>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<AggregatorRaycaster>()
                .AsSingle();
        }
    }
}
