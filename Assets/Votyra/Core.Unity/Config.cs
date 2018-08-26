using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Votyra.Core.Images;

namespace Votyra.Core
{

    [Serializable]
    public class ConfigItem : IEquatable<ConfigItem>
    {
        public ConfigItem()
        {
        }

        public ConfigItem(string id, Type type, object value)
        {
            Id = id;
            TypeAssemblyQualifiedName = type?.AssemblyQualifiedName;
            if (value is UnityEngine.Object)
            {
                UnityValue = value as UnityEngine.Object;
            }
            else
            {
                JsonValue = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            }
        }

        [SerializeField]
        public string Id;

        [SerializeField]
        public string TypeAssemblyQualifiedName;

        [SerializeField]
        public UnityEngine.Object UnityValue;

        [SerializeField]
        public string JsonValue;

        public Type Type
        {
            get
            {
                try
                {
                    return System.Type.GetType(TypeAssemblyQualifiedName);
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
                    return UnityValue != null ? UnityValue : Newtonsoft.Json.JsonConvert.DeserializeObject(JsonValue, Type);
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
            {
                return false;
            }
            return this.Id == that.Id && this.Type == that.Type && this.UnityValue == that.UnityValue && this.JsonValue == that.JsonValue;
        }

        public override bool Equals(object obj)
        {
            var that = obj as ConfigItem;
            return Equals(that);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}