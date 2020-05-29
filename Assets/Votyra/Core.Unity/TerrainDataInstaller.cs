using UniRx;
using UnityEngine;
using Votyra.Core.Logging;
using Votyra.Core.Profiling;
using Zenject;

namespace Votyra.Core.Unity
{
    public class TerrainDataInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            MainThreadDispatcher.Initialize();

            StaticLogger.LoggerImplementation = CreateLogger("StaticLogger", null);
            this.Container.Bind<IThreadSafeLogger>()
                .FromMethod(context =>
                {
                    //ObjectInstance can be null during constructor injection, but UnityEngine.Object do not support that. So they should be always set.
                    return new UnityLogger(context.ObjectType.FullName, context.ObjectInstance as Object);
                })
                .AsTransient();

            this.Container.Bind<IProfiler>()
                .FromMethod(context =>
                {
                    //ObjectInstance can be null during constructor injection, but UnityEngine.Object do not support that. So they should be always set.
                    return this.CreateProfiler(context.ObjectInstance as Object);
                })
                .AsTransient();
        }

        private static IThreadSafeLogger CreateLogger(string name, Object owner) => new UnityLogger(name, owner);

        private IProfiler CreateProfiler(Object owner) => new UnityProfiler(owner);
    }
}
