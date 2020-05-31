using Votyra.Core.Images;
using Votyra.Core.Images.Constraints;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class TerrainConfigsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<TerrainConfig>()
                .AsSingle();

            this.Container.BindInterfacesAndSelfTo<InterpolationConfig>()
                .AsSingle();

            this.Container.BindInterfacesAndSelfTo<ConstraintConfig>()
                .AsSingle();
        }
    }
}
