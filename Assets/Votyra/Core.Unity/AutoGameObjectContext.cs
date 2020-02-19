using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Votyra.Core.Unity
{
    [ExecuteInEditMode]
    public class AutoGameObjectContext : GameObjectContext
    {
        protected override void RunInternal()
        {
            AutoPopulateInstallers();
            base.RunInternal();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying)
            {
                AutoPopulateInstallers();
            }
        }
#endif

        private void AutoPopulateInstallers()
        {
            Installers = GetInstallers();
        }

        public IEnumerable<MonoInstaller> GetInstallers()
        {
            return GetComponents<MonoInstaller>()
                .Where(o => o.enabled);
        }
    }
}