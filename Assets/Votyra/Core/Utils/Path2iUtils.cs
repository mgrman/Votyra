using System;
using Votyra.Core.Models;

namespace Votyra.Core.Utils
{
    public static class Path2iUtils
    {
        public static void InvokeOnPath(Vector2i? from, Vector2i to, Action<Vector2i> action)
        {
            if (from == null)
            {
                action(to);
            }
            else if (from == to)
            {
            }
            else if (from.Value.X == to.X)
            {
                InvokeOnPathVertical(from.Value, to, action);
            }
            else if (from.Value.Y == to.Y)
            {
                InvokeOnPathHorizontal(from.Value, to, action);
            }
            else if (Math.Abs((to - from.Value).Y) < Math.Abs((to - from.Value).X))
            {
                InvokeOnPathHorizontalish(from.Value, to, action);
            }
            else
            {
                InvokeOnPathVerticalish(from.Value, to, action);
            }
        }

        private static void InvokeOnPathHorizontalish(Vector2i from, Vector2i to, Action<Vector2i> action)
        {
            float deltaX = to.X - from.X;
            float deltaY = to.Y - from.Y;
            var deltaErr = Math.Abs(deltaY / deltaX); // Assume deltaX != 0 (line is not vertical),
            // note that this division needs to be done in a way that preserves the fractional part
            var error = 0.0f;
            var y = from.Y;
            var signY = Math.Sign(to.X - from.X);
            for (var x = from.X; x != to.X; x += signY)
            {
                var position = new Vector2i(x, y);
                if (position != from)
                {
                    action(position);
                }

                error = error + deltaErr;
                if (!(error >= 0.5))
                {
                    continue;
                }

                y = y + Math.Sign(deltaY);
                error = error - 1.0f;
            }

            action(to);
        }

        private static void InvokeOnPathVerticalish(Vector2i from, Vector2i to, Action<Vector2i> action)
        {
            float deltaY = to.Y - from.Y;
            float deltaX = to.X - from.X;
            var deltaErr = Math.Abs(deltaX / deltaY); // Assume deltaY != 0 (line is not vertical),

            var error = 0.0f;
            var x = from.X;
            var signY = Math.Sign(to.Y - from.Y);

            for (var y = from.Y; y != to.Y; y += signY)
            {
                var position = new Vector2i(x, y);
                if (position != from)
                {
                    action(position);
                }

                error = error + deltaErr;
                if (!(error >= 0.5))
                {
                    continue;
                }

                x = x + Math.Sign(deltaX);
                error = error - 1.0f;
            }

            action(to);
        }

        private static void InvokeOnPathVertical(Vector2i from, Vector2i to, Action<Vector2i> action)
        {
            var x = from.X;
            var signY = Math.Sign(to.Y - from.Y);
            for (var y = from.Y + signY; y != to.Y; y += signY)
            {
                action(new Vector2i(x, y));
            }

            action(to);
        }

        private static void InvokeOnPathHorizontal(Vector2i from, Vector2i to, Action<Vector2i> action)
        {
            var y = from.Y;
            var signX = Math.Sign(to.X - from.X);
            for (var x = from.X + signX; x != to.X; x += signX)
            {
                action(new Vector2i(x, y));
            }

            action(to);
        }
    }
}