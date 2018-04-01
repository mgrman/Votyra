using Zenject;
using Votyra.Core.Profiling;
using Votyra.Core.Logging;

namespace Votyra.Core.Unity
{
    public class TerrainDataInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TerrainManagerModel>().AsSingle();

            Container.BindInterfacesAndSelfTo<StateModel>().AsSingle();

            Container.Bind<IThreadSafeLogger>()
                .FromMethod(context =>
                {
                    //ObjectInstance can be null during constructor injection, but UnityEngine.Object do not support that. So they should be always set.
                    return new UnityLogger(context.ObjectType.FullName, context.ObjectInstance as UnityEngine.Object);

                }).AsTransient();

            Container.Bind<IProfiler>()
                .FromMethod(context =>
                {
                    //ObjectInstance can be null during constructor injection, but UnityEngine.Object do not support that. So they should be always set.
                    return CreateProfiler(context.ObjectInstance as UnityEngine.Object);

                }).AsTransient();
        }

        private static IThreadSafeLogger CreateLogger(string name, UnityEngine.Object owner)
        {
            return new UnityLogger(name, owner);
        }

        private IProfiler CreateProfiler(UnityEngine.Object owner)
        {
            return new UnityProfiler(owner);
        }
    }
}