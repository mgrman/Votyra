using Votyra.Core.Images;
using Votyra.Core.Logging;

namespace Votyra.Core.Painting.Commands
{
    public class RemoveHoleLargeFactory : BaseFactory<RemoveHoleLarge>
    {
        public RemoveHoleLargeFactory(IEditableImage2f editableImage, IThreadSafeLogger logger)
            : base(editableImage, logger)
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

        protected override float Invoke(float value, int distance) => 0;
    }
}
