using Votyra.Core.Images;
using Votyra.Core.Logging;

namespace Votyra.Core.Painting.Commands
{
    public class IncreaseOrDecrease : PaintCommand
    {
        protected override float Invoke(float value, int strength) => value + strength;

        public IncreaseOrDecrease(IEditableImage2f editableImage, IThreadSafeLogger logger)
            : base(editableImage, logger)
        {
        }
    }
}