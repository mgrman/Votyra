using UniRx;
using UnityEngine;
using Votyra.Core.Logging;
using Votyra.Core.Profiling;
using Votyra.Core.Unity.Logging;
using Votyra.Core.Unity.Profiling;
using Zenject;

namespace Votyra.Core.Unity
{
    public class TerrainDataInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            MainThreadDispatcher.Initialize();
            Container.BindInterfacesAndSelfTo<TerrainManagerModel>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<StateModel>()
                .AsSingle();

            Container.Bind<IThreadSafeLogger>()
                .FromMethod(context =>
                {
                    //ObjectInstance can be null during constructor injection, but UnityEngine.Object do not support that. So they should be always set.
                    return new UnityLogger(context.ObjectType.FullName, context.ObjectInstance as Object);
                })
                .AsTransient();

            Container.Bind<IProfiler>()
                .FromMethod(context =>
                {
                    //ObjectInstance can be null during constructor injection, but UnityEngine.Object do not support that. So they should be always set.
                    return new UnityProfiler(context.ObjectInstance as Object);
                })
                .AsTransient();
        }
    }
}