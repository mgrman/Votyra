using System;
using UnityEngine;
using Zenject;

namespace Votyra.Core.Unity
{
    public class ConfigInstaller : MonoInstaller
    {
        [SerializeField]
        public ConfigItem[] Config;

        public override void InstallBindings()
        {
            try
            {
                if (this.Config == null)
                {
                    return;
                }

                foreach (var configItem in this.Config)
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
