using Votyra.Core.Images;
using Votyra.Core.Logging;

namespace Votyra.Core.Painting.Commands
{
    public class DecreaseLargeFactory : BaseFactory<DecreaseLarge>
    {
        public DecreaseLargeFactory(IEditableImage2f editableImage, IThreadSafeLogger logger)
            : base(editableImage, logger)
        {
        }

        public override string Action => KnownCommands.DecreaseLarge;
    }
}
