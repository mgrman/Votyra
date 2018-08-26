using UnityEngine;
using Votyra.Core.Models;
using Zenject;

namespace Votyra.Core.Images
{
    public class InitialImageConfig : IInitialImageConfig
    {
        [Inject]
        public InitialImageConfig([ConfigInject("initialData")]Texture2D initialData, [ConfigInject("initialDataScale")]Vector3f initialDataScale, [ConfigInject("zeroFromInitialStateIsNull")]bool zeroFromInitialStateIsNull)
        {
            InitialData = initialData;
            InitialDataScale = initialDataScale;
            ZeroFromInitialStateIsNull = zeroFromInitialStateIsNull;
        }

        public object InitialData { get; }
        public Vector3f InitialDataScale { get; }
        public bool ZeroFromInitialStateIsNull { get; }

        public static bool operator ==(InitialImageConfig a, InitialImageConfig b)
        {
            return a?.Equals(b) ?? b?.Equals(a) ?? true;
        }

        public static bool operator !=(InitialImageConfig a, InitialImageConfig b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var that = obj as InitialImageConfig;

            return this.InitialData == that.InitialData && this.InitialDataScale == that.InitialDataScale;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.InitialData?.GetHashCode() ?? 0 + this.InitialDataScale.GetHashCode() * 7;
            }
        }
    }
}