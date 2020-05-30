using Votyra.Core.Images;
using Votyra.Core.Logging;

namespace Votyra.Core.Painting.Commands
{
    public class IncreaseFactory : BaseFactory<Increase>
    {
        public IncreaseFactory(IEditableImage2f editableImage, IThreadSafeLogger logger)
            : base(editableImage, logger)
        {
        }

        public override string Action => KnownCommands.Increase;
    }
}
