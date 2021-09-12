using System;
using UnityEngine;

namespace Votyra.Core.Models
{
    [Serializable]
    public struct UIVector3f
    {
        public float x;
        public float y;
        public float z;

        public UIVector3f(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vector3(UIVector3f value) => new Vector3(value.x, value.y, value.z);

        public static implicit operator UIVector3f(Vector3 value) => new UIVector3f(value.x, value.y, value.z);

        public static implicit operator Vector3f(UIVector3f value) => new Vector3f(value.x, value.y, value.z);

        public static implicit operator UIVector3f(Vector3f value) => new UIVector3f(value.X, value.Y, value.Z);

        public static bool operator ==(UIVector3f a, UIVector3f b) => a.x == b.x && a.y == b.y && a.z == b.z;

        public static bool operator !=(UIVector3f a, UIVector3f b) => a.x != b.x || a.y != b.y || a.z != b.z;

        public override bool Equals(object obj)
        {
            if (!(obj is UIVector3f))
                return false;
            var that = (UIVector3f) obj;

            return this == that;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return x.GetHashCode() + y.GetHashCode() * 7 + z.GetHashCode() * 13;
            }
        }
    }
}