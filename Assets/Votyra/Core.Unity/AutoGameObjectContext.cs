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
            this.AutoPopulateInstallers();
            base.RunInternal();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying)
            {
                this.AutoPopulateInstallers();
            }
        }
#endif

        private void AutoPopulateInstallers()
        {
            this.Installers = this.GetInstallers();
        }

        public IEnumerable<MonoInstaller> GetInstallers()
        {
            return this.GetComponents<MonoInstaller>()
                .Where(o => o.enabled);
        }
    }
}
