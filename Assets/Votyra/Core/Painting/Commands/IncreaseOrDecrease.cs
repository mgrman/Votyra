using Votyra.Core.Images;
using Votyra.Core.Logging;

namespace Votyra.Core.Painting.Commands
{
    public class IncreaseOrDecrease : PaintCommand
    {
        public IncreaseOrDecrease(IEditableImage2f editableImage, IEditableMask2e editableMask, IThreadSafeLogger logger)
            : base(editableImage, editableMask, logger)
        {
        }

        protected override float Invoke(float value, int strength) => value + strength;
    }
}