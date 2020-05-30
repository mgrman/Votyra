using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
using Votyra.Core.Utils;
using UnityEngineObject = UnityEngine.Object;

namespace Votyra.Core
{
    [Serializable]
    public class ConfigItem : IEquatable<ConfigItem>
    {
        private JsonSerializerSettings settings;

        [FormerlySerializedAs("Id")]
        [SerializeField]
        private string id;

        [FormerlySerializedAs("JsonValue")]
        [SerializeField]
        private string jsonValue;

        [FormerlySerializedAs("TypeAssemblyQualifiedName")]
        [SerializeField]
        private string typeAssemblyQualifiedName;

        [FormerlySerializedAs("UnityValues")]
        [SerializeField]
        private List<UnityEngineObject> unityValues;

        public string Id => id;

        public string JsonValue => jsonValue;

        public string TypeAssemblyQualifiedName => typeAssemblyQualifiedName;

        public List<UnityEngineObject> UnityValues => unityValues;

        public ConfigItem()
        {
            this.settings = new JsonSerializerSettings();
            this.settings.Converters.Add(new ObjectConverter(this));
        }

        public ConfigItem(string id, Type type, object value)
            : this()
        {
            this.id = id;
            this.typeAssemblyQualifiedName = type?.AssemblyQualifiedName;

            this.jsonValue = JsonConvert.SerializeObject(value, this.settings);
        }

        public Type Type
        {
            get
            {
                try
                {
                    return Type.GetType(this.typeAssemblyQualifiedName);
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
                    return JsonConvert.DeserializeObject(this.jsonValue, this.Type, this.settings);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Problem '{ex.Message}' deserializing '{this.jsonValue}' to type {this.Type} for {this.id}.");
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

            return (this.id == that.id) && (this.Type == that.Type) && this.unityValues.EmptyIfNull()
                .SequenceEqual(that.unityValues.EmptyIfNull()) && (this.jsonValue == that.jsonValue);
        }

        public override bool Equals(object obj)
        {
            var that = obj as ConfigItem;
            return this.Equals(that);
        }

        public override int GetHashCode() => this.id.GetHashCode();

        public override string ToString() => $"CONFIG {this.id}({this.Type.Name}): {this.jsonValue}";

        public class ObjectConverter : JsonConverter
        {
            private readonly ConfigItem parent;

            public ObjectConverter(ConfigItem parent)
            {
                this.parent = parent;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(this.parent.unityValues?.Count ?? 0);

                this.parent.unityValues = this.parent.unityValues ?? new List<UnityEngineObject>();
                this.parent.unityValues.Add(value as UnityEngineObject);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.ValueType == typeof(long))
                {
                    var index = (int)(long)reader.Value;
                    var obj = this.parent.unityValues[index];
                    return obj == null ? null : obj;
                }

                return null;
            }

            public override bool CanConvert(Type objectType) => typeof(UnityEngineObject).IsAssignableFrom(objectType);
        }
    }
}
