using Votyra.Core.Images;
using Votyra.Core.Logging;

namespace Votyra.Core.Painting.Commands
{
    public class DecreaseLargeFactory : BaseFactory<DecreaseLarge>
    {
        public DecreaseLargeFactory(IEditableImage2f editableImage, IThreadSafeLogger logger)
            : base(editableImage,  logger)
        {
        }

        public override string Action => KnownCommands.DecreaseLarge;
    }

    public class DecreaseLarge : PaintCommand
    {
        public DecreaseLarge()
            : base(2)
        {
        }

        protected override float Invoke(float value, int distance) => value - 2 + distance;
    }
}