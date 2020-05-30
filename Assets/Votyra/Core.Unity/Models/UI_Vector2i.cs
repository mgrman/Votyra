using System;

namespace Votyra.Core.Models
{
    [Serializable]
    public struct UiVector2i
    {
        public int x;
        public int y;

        public UiVector2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator Vector2i(UiVector2i value) => new Vector2i(value.x, value.y);

        public static implicit operator UiVector2i(Vector2i value) => new UiVector2i(value.X, value.Y);

        public static bool operator ==(UiVector2i a, UiVector2i b) => (a.x == b.x) && (a.y == b.y);

        public static bool operator !=(UiVector2i a, UiVector2i b) => (a.x != b.x) || (a.y != b.y);

        public override bool Equals(object obj)
        {
            if (!(obj is UiVector2i))
            {
                return false;
            }

            var that = (UiVector2i)obj;

            return this == that;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.x + (this.y * 7);
            }
        }
    }
}
