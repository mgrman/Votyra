using Votyra.Core.Images;
using Votyra.Core.Logging;

namespace Votyra.Core.Painting.Commands
{
    public class MakeHoleFactory : BaseFactory<MakeHole>
    {
        public MakeHoleFactory(IEditableImage2f editableImage, IThreadSafeLogger logger)
            : base(editableImage, logger)
        {
        }

        public override string Action => KnownCommands.MakeHole;
    }
}
