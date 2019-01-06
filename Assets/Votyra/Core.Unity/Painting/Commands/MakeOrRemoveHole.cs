using Votyra.Core.Models;

namespace Votyra.Core.Painting.Commands
{
    public class MakeOrRemoveHole : PaintCommand
    {
        protected override MaskValues Invoke(MaskValues value, int strength) => strength < 0f ? MaskValues.Hole : MaskValues.Terrain;
    }
}