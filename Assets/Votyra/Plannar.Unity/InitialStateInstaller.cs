using Votyra.Core.Images;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class InitialStateInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<InitialStateSetter2f>()
                .AsSingle()
                .NonLazy();
            Container.BindInterfacesAndSelfTo<InitialImageConfig>()
                .AsSingle();
        }
    }
}