using Votyra.Core.Images;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class MaterialConfigInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<MaterialConfig>()
                .AsSingle();
        }
    }
}
