using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.Models;

namespace Votyra.Core.Painting.Commands
{
    public class MakeHoleLargeFactory : BaseFactory<MakeHoleLarge>
    {
        public MakeHoleLargeFactory(IEditableImage2f editableImage, IThreadSafeLogger logger)
            : base(editableImage, logger)
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

        protected override float Invoke(float value, int distance) => float.NaN;
    }
}
