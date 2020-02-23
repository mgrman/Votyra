using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Unity
{
    public class TerrainDataConfigInstaller : MonoInstaller
    {
        [SerializeField]
        public ConfigItem[] Config;

        public override void InstallBindings()
        {

            try
            {
                if (Config != null)
                    foreach (var configItem in Config)
                    {
                        var type = configItem.Type;
                        var value = configItem.Value;

                        Container.Bind(type)
                            .WithId(configItem.Id)
                            .FromInstance(value);
                    }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex, this);
            }
        }
    }
}