using Votyra.Core.Images;
using Votyra.Core.Logging;

namespace Votyra.Core.Painting.Commands
{
    public class RemoveHoleLargeFactory : BaseFactory<RemoveHoleLarge>
    {
        public RemoveHoleLargeFactory(IEditableImage2F editableImage, IThreadSafeLogger logger)
            : base(editableImage, logger)
        {
        }

        public override string Action => KnownCommands.RemoveHoleLarge;
    }
}
