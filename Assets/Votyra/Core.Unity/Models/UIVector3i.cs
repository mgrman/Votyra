using System;

namespace Votyra.Core.Models
{
    [Serializable]
    public struct UIVector3i
    {
        public int x;
        public int y;
        public int z;

        public UIVector3i(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vector3i(UIVector3i value) => new Vector3i(value.x, value.y, value.z);

        public static implicit operator UIVector3i(Vector3i value) => new UIVector3i(value.X, value.Y, value.Z);

        public static bool operator ==(UIVector3i a, UIVector3i b) => a.x == b.x && a.y == b.y && a.z == b.z;

        public static bool operator !=(UIVector3i a, UIVector3i b) => a.x != b.x || a.y != b.y || a.z != b.z;

        public override bool Equals(object obj)
        {
            if (!(obj is UIVector3i))
                return false;
            var that = (UIVector3i) obj;

            return this == that;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return x + y * 7 + z * 13;
            }
        }
    }
}