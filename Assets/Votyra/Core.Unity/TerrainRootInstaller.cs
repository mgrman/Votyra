using UniRx;
using UnityEngine;
using Votyra.Core.Logging;
using Votyra.Core.Profiling;
using Zenject;

namespace Votyra.Core.Unity
{
    public class TerrainRootInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var meshRoot = new GameObject("MeshRoot");
            meshRoot.transform.SetParent(transform, false);
            Container.BindInstance(meshRoot)
                .WithId("root")
                .AsSingle();
        }
    }
}