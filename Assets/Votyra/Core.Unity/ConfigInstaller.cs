using System;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Votyra.Core.Unity
{
    public class ConfigInstaller : MonoInstaller
    {
        [FormerlySerializedAs("Config")]
        [SerializeField]
        private ConfigItem[] config;

        public ConfigItem[] Config
        {
            get => this.config;
            set => this.config = value;
        }

        public override void InstallBindings()
        {
            try
            {
                if (this.config == null)
                {
                    return;
                }

                foreach (var configItem in this.config)
                {
                    var type = configItem.Type;
                    try
                    {
                        var value = configItem.Value;

                        this.Container.Bind(type)
                            .WithId(configItem.Id)
                            .FromInstance(value);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex, this);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex, this);
            }
        }
    }
}
