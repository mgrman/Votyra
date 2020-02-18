using Votyra.Core.Images;
using Votyra.Core.Logging;

namespace Votyra.Core.Painting.Commands
{
    public class DecreaseFactory : BaseFactory<Decrease>
    {
        public DecreaseFactory(IEditableImage2f editableImage, IEditableMask2e editableMask, IThreadSafeLogger logger)
            : base(editableImage, editableMask, logger)
        {
        }

        public override string Action => KnownCommands.Decrease;
    }

    public class Decrease : PaintCommand
    {
        public Decrease()
            : base(0)
        {
        }

        protected override float Invoke(float value, int distance) => value - 1;
    }
}