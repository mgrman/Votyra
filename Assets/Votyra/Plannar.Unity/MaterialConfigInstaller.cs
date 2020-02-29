using Votyra.Core.Images;
using Votyra.Core.Images.Constraints;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class MaterialConfigInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MaterialConfig>()
                .AsSingle();
        }

    }
}