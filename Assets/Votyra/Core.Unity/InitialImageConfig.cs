using UnityEngine;
using Votyra.Core.Models;
using Zenject;

namespace Votyra.Core.Images
{
    public class InitialImageConfig : IInitialImageConfig
    {
        [Inject]
        public InitialImageConfig([ConfigInject("initialData")] Texture2D initialData, [ConfigInject("initialDataScale")] Vector3f initialDataScale)
        {
            InitialData = initialData;
            InitialDataScale = initialDataScale;
        }

        public object InitialData { get; }
        
        public Vector3f InitialDataScale { get; }

        public static bool operator ==(InitialImageConfig a, InitialImageConfig b) => a?.Equals(b) ?? b?.Equals(a) ?? true;

        public static bool operator !=(InitialImageConfig a, InitialImageConfig b) => !(a == b);

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var that = obj as InitialImageConfig;

            return InitialData == that.InitialData && InitialDataScale == that.InitialDataScale;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return InitialData?.GetHashCode() ?? 0 + InitialDataScale.GetHashCode() * 7;
            }
        }
    }
}