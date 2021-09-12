using System;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Votyra.Core.Unity.Config
{
    [Serializable]
    public class ConfigItem : IEquatable<ConfigItem>
    {
        [SerializeField]
        public string Id;

        [SerializeField]
        public string JsonValue;

        [SerializeField]
        public string TypeAssemblyQualifiedName;

        [SerializeField]
        public Object UnityValue;

        public ConfigItem()
        {
        }

        public ConfigItem(string id, Type type, object value)
        {
            Id = id;
            TypeAssemblyQualifiedName = type?.AssemblyQualifiedName;
            if (value is Object)
                UnityValue = value as Object;
            else
                JsonValue = JsonConvert.SerializeObject(value);
        }

        public Type Type
        {
            get
            {
                try
                {
                    return Type.GetType(TypeAssemblyQualifiedName);
                }
                catch
                {
                    return null;
                }
            }
        }

        public object Value
        {
            get
            {
                try
                {
                    return UnityValue != null ? UnityValue : JsonConvert.DeserializeObject(JsonValue, Type);
                }
                catch
                {
                    return null;
                }
            }
        }

        public bool Equals(ConfigItem that)
        {
            if (that == null)
                return false;
            return Id == that.Id && Type == that.Type && UnityValue == that.UnityValue && JsonValue == that.JsonValue;
        }

        public override bool Equals(object obj)
        {
            var that = obj as ConfigItem;
            return Equals(that);
        }

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => $"CONFIG {Id}({Type.Name}): {JsonValue}{UnityValue}";
    }
}