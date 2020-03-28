using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.Models;

namespace Votyra.Core.Painting.Commands
{
    public class RemoveHoleLargeFactory : BaseHoleFactory<RemoveHoleLarge>
    {
        public RemoveHoleLargeFactory(IEditableImage2f editableImage, IEditableMask2e editableMask, IThreadSafeLogger logger)
            : base(editableImage, editableMask, logger)
        {
        }

        public override string Action => KnownCommands.RemoveHoleLarge;
    }

    public class RemoveHoleLarge : HolePaintCommand
    {
        public RemoveHoleLarge()
            : base(2)
        {
        }

        protected override MaskValues Invoke(MaskValues value, int distance) => MaskValues.Terrain;
    }
}