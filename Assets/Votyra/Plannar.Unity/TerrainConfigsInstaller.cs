using Votyra.Core.Images;
using Votyra.Core.Images.Constraints;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class TerrainConfigsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TerrainConfig>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<InterpolationConfig>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<ConstraintConfig>()
                .AsSingle();
        }

    }
}