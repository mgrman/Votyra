using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.Models;

namespace Votyra.Core.Painting.Commands
{
    public class MakeHoleFactory : BaseFactory<MakeHole>
    {
        public MakeHoleFactory(IEditableImage2f editableImage, IEditableMask2e editableMask, IThreadSafeLogger logger)
            : base(editableImage, editableMask, logger)
        {
        }

        public override string Action => KnownCommands.MakeHole;
    }

    public class MakeHole : PaintCommand
    {
        public MakeHole()
            : base(0)
        {
        }

        protected override MaskValues Invoke(MaskValues value, int distance) => MaskValues.Hole;
    }
}