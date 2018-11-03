using System.Collections.Generic;
using UniRx;
using Votyra.Core.Models;
using Votyra.Core.Painting.Commands;

namespace Votyra.Core.Painting
{
    public struct PaintInvocationData
    {
        public PaintInvocationData(int strength, Vector2i imagePosition) : this()
        {
            Strength = strength;
            ImagePosition = imagePosition;
        }

        public readonly int Strength;
        public readonly Vector2i ImagePosition;
    }
}