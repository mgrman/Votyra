using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Votyra.Core
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
        public Object[] UnityValues;

        private JsonSerializerSettings _settings;

        public ConfigItem()
        {
            _settings = new JsonSerializerSettings();
            _settings.Converters.Add(new ObjectConverter(this));
        }

        public ConfigItem(string id, Type type, object value)
            : this()
        {
            Id = id;
            TypeAssemblyQualifiedName = type?.AssemblyQualifiedName;

            JsonValue = JsonConvert.SerializeObject(value, _settings);
        }

        public class ObjectConverter : JsonConverter
        {
            private ConfigItem _parent;

            public ObjectConverter(ConfigItem parent)
            {
                _parent = parent;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(_parent.UnityValues?.Length ?? 0);
                _parent.UnityValues = _parent.UnityValues ?? Array.Empty<UnityEngine.Object>();
                _parent.UnityValues = _parent.UnityValues.Concat(new[] {value as UnityEngine.Object})
                    .ToArray();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                try
                {
                    if (reader.ValueType == typeof(long))
                    {
                        var index = (long) reader.Value;
                        return _parent.UnityValues[index];
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    throw;
                }
            }

            public override bool CanConvert(Type objectType) => typeof(UnityEngine.Object).IsAssignableFrom(objectType);
        }

        public Type Type
        {
            get
            {
                try
                {
                    return Type.GetType(TypeAssemblyQualifiedName);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
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
                    return JsonConvert.DeserializeObject(JsonValue, Type, _settings);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    return null;
                }
            }
        }

        public bool Equals(ConfigItem that)
        {
            if (that == null)
                return false;
            return Id == that.Id && Type == that.Type && (UnityValues ?? Array.Empty<UnityEngine.Object>()).SequenceEqual(that.UnityValues ?? Array.Empty<UnityEngine.Object>()) && JsonValue == that.JsonValue;
        }

        public override bool Equals(object obj)
        {
            var that = obj as ConfigItem;
            return Equals(that);
        }

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => $"CONFIG {Id}({Type.Name}): {JsonValue}";
    }
}