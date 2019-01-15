using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.Models;

namespace Votyra.Core.Painting.Commands
{
    public class MakeOrRemoveHole : PaintCommand
    {
        protected override MaskValues Invoke(MaskValues value, int strength) => strength < 0f ? MaskValues.Hole : MaskValues.Terrain;

        public MakeOrRemoveHole(IEditableImage2f editableImage, IEditableMask2e editableMask, IThreadSafeLogger logger)
            : base(editableImage, editableMask, logger)
        {
        }
    }
}