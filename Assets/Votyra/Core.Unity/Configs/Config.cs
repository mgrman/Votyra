using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Votyra.Core.Utils;
using UnityEngineObject = UnityEngine.Object;

namespace Votyra.Core
{
    [Serializable]
    public class ConfigItem : IEquatable<ConfigItem>
    {
        private JsonSerializerSettings _settings;

        [SerializeField]
        public string Id;

        [SerializeField]
        public string JsonValue;

        [SerializeField]
        public string TypeAssemblyQualifiedName;

        [SerializeField]
        public List<UnityEngineObject> UnityValues;

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
                    Debug.LogError($"Problem '{ex.Message}' deserializing '{JsonValue}' to type {Type} for {Id}.");
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

            return Id == that.Id && Type == that.Type && UnityValues.EmptyIfNull()
                .SequenceEqual(that.UnityValues.EmptyIfNull()) && JsonValue == that.JsonValue;
        }

        public override bool Equals(object obj)
        {
            var that = obj as ConfigItem;
            return Equals(that);
        }

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => $"CONFIG {Id}({Type.Name}): {JsonValue}";

        public class ObjectConverter : JsonConverter
        {
            private readonly ConfigItem _parent;

            public ObjectConverter(ConfigItem parent)
            {
                _parent = parent;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(_parent.UnityValues?.Count ?? 0);

                _parent.UnityValues = _parent.UnityValues ?? new List<UnityEngineObject>();
                _parent.UnityValues.Add(value as UnityEngineObject);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.ValueType == typeof(long))
                {
                    var index = (int) (long) reader.Value;
                    var obj = _parent.UnityValues[index];
                    return obj == null ? null : obj;
                }

                return null;
            }

            public override bool CanConvert(Type objectType) => typeof(UnityEngineObject).IsAssignableFrom(objectType);
        }
    }
}
