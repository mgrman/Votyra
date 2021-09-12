using System;
using Votyra.Core.Models;

namespace Votyra.Core.Unity.Models
{
    [Serializable]
    public struct UIVector2i
    {
        public int x;
        public int y;

        public UIVector2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator Vector2i(UIVector2i value) => new Vector2i(value.x, value.y);

        public static implicit operator UIVector2i(Vector2i value) => new UIVector2i(value.X, value.Y);

        public static bool operator ==(UIVector2i a, UIVector2i b) => a.x == b.x && a.y == b.y;

        public static bool operator !=(UIVector2i a, UIVector2i b) => a.x != b.x || a.y != b.y;

        public override bool Equals(object obj)
        {
            if (!(obj is UIVector2i))
                return false;
            var that = (UIVector2i) obj;

            return this == that;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return x + y * 7;
            }
        }
    }
}