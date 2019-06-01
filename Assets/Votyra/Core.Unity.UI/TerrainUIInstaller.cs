using System;
using UnityEngine;
using Votyra.Core.Unity.Painting;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class TerrainUIInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UnityInputManager>()
                .FromNewComponentOn(c => c.Container.ResolveId<GameObject>("root"))
                .AsSingle()
                .NonLazy();
        }
    }
}