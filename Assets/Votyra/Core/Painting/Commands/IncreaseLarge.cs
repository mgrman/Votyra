using Votyra.Core.Images;
using Votyra.Core.Logging;

namespace Votyra.Core.Painting.Commands
{
    public class IncreaseLargeFactory : BaseFactory<IncreaseLarge>
    {
        public IncreaseLargeFactory(IEditableImage2f editableImage, IEditableMask2e editableMask, IThreadSafeLogger logger)
            : base(editableImage, editableMask, logger)
        {
        }

        public override string Action => KnownCommands.IncreaseLarge;
    }

    public class IncreaseLarge : PaintCommand
    {
        public IncreaseLarge()
            : base(2)
        {
        }

        protected override float Invoke(float value, int distance) => value + 2 - distance;
    }
}