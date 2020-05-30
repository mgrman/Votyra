using System;
using UnityEngine;

namespace Votyra.Core.Models
{
    [Serializable]
    public struct UiVector3F
    {
        public float x;
        public float y;
        public float z;

        public UiVector3F(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vector3(UiVector3F value) => new Vector3(value.x, value.y, value.z);

        public static implicit operator UiVector3F(Vector3 value) => new UiVector3F(value.x, value.y, value.z);

        public static implicit operator Vector3f(UiVector3F value) => new Vector3f(value.x, value.y, value.z);

        public static implicit operator UiVector3F(Vector3f value) => new UiVector3F(value.X, value.Y, value.Z);

        public static bool operator ==(UiVector3F a, UiVector3F b) => (a.x == b.x) && (a.y == b.y) && (a.z == b.z);

        public static bool operator !=(UiVector3F a, UiVector3F b) => (a.x != b.x) || (a.y != b.y) || (a.z != b.z);

        public override bool Equals(object obj)
        {
            if (!(obj is UiVector3F))
            {
                return false;
            }

            var that = (UiVector3F)obj;

            return this == that;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.x.GetHashCode() + (this.y.GetHashCode() * 7) + (this.z.GetHashCode() * 13);
            }
        }
    }
}
