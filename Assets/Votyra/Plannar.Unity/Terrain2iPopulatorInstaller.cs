using Zenject;

namespace Votyra.Plannar.Unity
{
    public class Terrain2iPopulatorInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<PopulatorConfig>()
                .AsSingle();

            this.Container.BindInterfacesAndSelfTo<TerrainPopulatorManager>()
                .FromNewComponentOn(this.gameObject)
                .AsSingle()
                .NonLazy();
        }
    }
}
