using System;

namespace Votyra.Core.Models
{
    [Serializable]
    public struct UiVector2I
    {
        public int x;
        public int y;

        public UiVector2I(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator Vector2i(UiVector2I value) => new Vector2i(value.x, value.y);

        public static implicit operator UiVector2I(Vector2i value) => new UiVector2I(value.X, value.Y);

        public static bool operator ==(UiVector2I a, UiVector2I b) => (a.x == b.x) && (a.y == b.y);

        public static bool operator !=(UiVector2I a, UiVector2I b) => (a.x != b.x) || (a.y != b.y);

        public override bool Equals(object obj)
        {
            if (!(obj is UiVector2I))
            {
                return false;
            }

            var that = (UiVector2I)obj;

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
