using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.Models;

namespace Votyra.Core.Painting.Commands
{
    public class MakeHoleLargeFactory : BaseHoleFactory<MakeHoleLarge>
    {
        public MakeHoleLargeFactory(IEditableImage2f editableImage, IEditableMask2e editableMask, IThreadSafeLogger logger)
            : base(editableImage, editableMask, logger)
        {
        }

        public override string Action => KnownCommands.MakeHoleLarge;
    }

    public class MakeHoleLarge : HolePaintCommand
    {
        public MakeHoleLarge()
            : base(2)
        {
        }

        protected override MaskValues Invoke(MaskValues value, int distance) => MaskValues.Hole;
    }
}