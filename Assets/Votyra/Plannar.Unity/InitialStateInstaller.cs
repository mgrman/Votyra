using Votyra.Core.Images;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class InitialStateInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<InitialStateSetter2f>()
                .AsSingle()
                .NonLazy();
            this.Container.BindInterfacesAndSelfTo<InitialImageConfig>()
                .AsSingle();
        }
    }
}
