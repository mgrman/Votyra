using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.Models;

namespace Votyra.Core.Painting.Commands
{
    public class RemoveHoleFactory : BaseFactory<RemoveHole>
    {
        public RemoveHoleFactory(IEditableImage2f editableImage, IEditableMask2e editableMask, IThreadSafeLogger logger)
            : base(editableImage, editableMask, logger)
        {
        }

        public override string Action => KnownCommands.RemoveHole;
    }

    public class RemoveHole : PaintCommand
    {
        public RemoveHole()
            : base(1)
        {
        }

        protected override MaskValues Invoke(MaskValues value, int distance) => MaskValues.Terrain;
    }
}