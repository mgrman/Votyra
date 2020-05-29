using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.Models;

namespace Votyra.Core.Painting.Commands
{
    public class RemoveHoleFactory : BaseFactory<RemoveHole>
    {
        public RemoveHoleFactory(IEditableImage2f editableImage, IThreadSafeLogger logger)
            : base(editableImage,  logger)
        {
        }

        public override string Action => KnownCommands.RemoveHole;
    }

    public class RemoveHole : HolePaintCommand
    {
        public RemoveHole()
            : base(1)
        {
        }

        protected override float Invoke(float value, int distance) => 0;
    }
}
