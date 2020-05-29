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
            this._settings = new JsonSerializerSettings();
            this._settings.Converters.Add(new ObjectConverter(this));
        }

        public ConfigItem(string id, Type type, object value)
            : this()
        {
            this.Id = id;
            this.TypeAssemblyQualifiedName = type?.AssemblyQualifiedName;

            this.JsonValue = JsonConvert.SerializeObject(value, this._settings);
        }

        public Type Type
        {
            get
            {
                try
                {
                    return Type.GetType(this.TypeAssemblyQualifiedName);
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
                    return JsonConvert.DeserializeObject(this.JsonValue, this.Type, this._settings);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Problem '{ex.Message}' deserializing '{this.JsonValue}' to type {this.Type} for {this.Id}.");
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

            return (this.Id == that.Id) && (this.Type == that.Type) && this.UnityValues.EmptyIfNull()
                .SequenceEqual(that.UnityValues.EmptyIfNull()) && (this.JsonValue == that.JsonValue);
        }

        public override bool Equals(object obj)
        {
            var that = obj as ConfigItem;
            return this.Equals(that);
        }

        public override int GetHashCode() => this.Id.GetHashCode();

        public override string ToString() => $"CONFIG {this.Id}({this.Type.Name}): {this.JsonValue}";

        public class ObjectConverter : JsonConverter
        {
            private readonly ConfigItem _parent;

            public ObjectConverter(ConfigItem parent)
            {
                this._parent = parent;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(this._parent.UnityValues?.Count ?? 0);

                this._parent.UnityValues = this._parent.UnityValues ?? new List<UnityEngineObject>();
                this._parent.UnityValues.Add(value as UnityEngineObject);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.ValueType == typeof(long))
                {
                    var index = (int)(long)reader.Value;
                    var obj = this._parent.UnityValues[index];
                    return obj == null ? null : obj;
                }

                return null;
            }

            public override bool CanConvert(Type objectType) => typeof(UnityEngineObject).IsAssignableFrom(objectType);
        }
    }
}
