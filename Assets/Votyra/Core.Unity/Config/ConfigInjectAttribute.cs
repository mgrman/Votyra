using System;
using Zenject;

namespace Votyra.Core.Unity.Config
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field)]
    public class ConfigInjectAttribute : InjectOptionalAttribute
    {
        public ConfigInjectAttribute(string id)
        {
            Id = id;
        }
    }
}