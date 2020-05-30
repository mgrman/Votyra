using System;

namespace Votyra.Core.Models
{
    [Serializable]
    public struct UiVector3I
    {
        public int x;
        public int y;
        public int z;

        public UiVector3I(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vector3i(UiVector3I value) => new Vector3i(value.x, value.y, value.z);

        public static implicit operator UiVector3I(Vector3i value) => new UiVector3I(value.X, value.Y, value.Z);

        public static bool operator ==(UiVector3I a, UiVector3I b) => (a.x == b.x) && (a.y == b.y) && (a.z == b.z);

        public static bool operator !=(UiVector3I a, UiVector3I b) => (a.x != b.x) || (a.y != b.y) || (a.z != b.z);

        public override bool Equals(object obj)
        {
            if (!(obj is UiVector3I))
            {
                return false;
            }

            var that = (UiVector3I)obj;

            return this == that;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.x + (this.y * 7) + (this.z * 13);
            }
        }
    }
}
