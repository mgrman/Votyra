using System;

namespace Votyra.Core.Models
{
    [Serializable]
    public struct UI_Vector2i
    {
        public int x;
        public int y;

        public UI_Vector2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator Vector2i(UI_Vector2i value)
        {
            return new Vector2i(value.x, value.y);
        }

        public static implicit operator UI_Vector2i(Vector2i value)
        {
            return new UI_Vector2i(value.X, value.Y);
        }

        public static bool operator ==(UI_Vector2i a, UI_Vector2i b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(UI_Vector2i a, UI_Vector2i b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is UI_Vector2i))
            {
                return false;
            }
            var that = (UI_Vector2i)obj;

            return this == that;
        }

        public override int GetHashCode()
        {
            return x + y * 7;
        }
    }
}