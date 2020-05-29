using Votyra.Core.Models;

namespace Votyra.Core.Painting
{
    public struct PaintInvocationData
    {
        public PaintInvocationData(int strength, Vector2i imagePosition)
            : this()
        {
            this.Strength = strength;
            this.ImagePosition = imagePosition;
        }

        public readonly int Strength;
        public readonly Vector2i ImagePosition;
    }
}
