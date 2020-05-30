using Votyra.Core.Images;
using Votyra.Core.Logging;

namespace Votyra.Core.Painting.Commands
{
    public class MakeHoleLargeFactory : BaseFactory<MakeHoleLarge>
    {
        public MakeHoleLargeFactory(IEditableImage2F editableImage, IThreadSafeLogger logger)
            : base(editableImage, logger)
        {
        }

        public override string Action => KnownCommands.MakeHoleLarge;
    }
}
