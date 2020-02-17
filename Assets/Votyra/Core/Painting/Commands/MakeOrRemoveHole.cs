using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.Models;

namespace Votyra.Core.Painting.Commands
{
    public class MakeOrRemoveHole : PaintCommand
    {
        public MakeOrRemoveHole(IEditableImage2f editableImage, IEditableMask2e editableMask, IThreadSafeLogger logger)
            : base(editableImage, editableMask, logger)
        {
        }

        protected override MaskValues Invoke(MaskValues value, int strength)
        {
            if (strength < 0)
            {
                return MaskValues.Hole;
            }

            if (strength > 0)
            {
                return MaskValues.Terrain;
            }
            return value;
        }
    }
}