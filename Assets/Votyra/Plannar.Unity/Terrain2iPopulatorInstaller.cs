using Zenject;

namespace Votyra.Plannar.Unity
{
    public class Terrain2iPopulatorInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PopulatorConfig>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<TerrainPopulatorManager>()
                .FromNewComponentOn(this.gameObject)
                .AsSingle()
                .NonLazy();
        }
    }
}