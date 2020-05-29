using Votyra.Core.Raycasting;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class RaycastingTerrainUnityInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<SingleRaycaster>()
                .AsSingle()
                .NonLazy();
        }
    }
}
