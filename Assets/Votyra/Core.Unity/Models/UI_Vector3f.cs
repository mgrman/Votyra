using System;
using UnityEngine;

namespace Votyra.Core.Models
{
    [Serializable]
    public struct UI_Vector3f
    {
        public float x;
        public float y;
        public float z;

        public UI_Vector3f(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator UI_Vector3f(Vector3 value)
        {
            return new UI_Vector3f(value.x, value.y, value.z);
        }

        public static implicit operator UI_Vector3f(Vector3f value)
        {
            return new UI_Vector3f(value.X, value.Y, value.Z);
        }

        public static implicit operator Vector3(UI_Vector3f value)
        {
            return new Vector3(value.x, value.y, value.z);
        }

        public static implicit operator Vector3f(UI_Vector3f value)
        {
            return new Vector3f(value.x, value.y, value.z);
        }

        public static bool operator !=(UI_Vector3f a, UI_Vector3f b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.z;
        }

        public static bool operator ==(UI_Vector3f a, UI_Vector3f b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is UI_Vector3f))
            {
                return false;
            }
            var that = (UI_Vector3f)obj;

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