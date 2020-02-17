using Votyra.Core.Images;
using Votyra.Core.Logging;

namespace Votyra.Core.Painting.Commands
{
    public class FlattenFactory : BaseFactory<Flatten>
    {
        public FlattenFactory(IEditableImage2f editableImage, IEditableMask2e editableMask, IThreadSafeLogger logger)
            : base(editableImage, editableMask, logger)
        {
        }

        public override string Action => KnownCommands.Flatten;
    }

    public class Flatten : PaintCommand
    {
        private const float smoothSpeedRelative = 0.2f;
        private float? _centerValue;

        public Flatten()
            : base(0)
        {
        }

        protected override void OnInvocationStopping()
        {
            base.OnInvocationStopping();
            _centerValue = null;
        }

        protected override void PrepareWithClickedValue(float clickedValue)
        {
            _centerValue = _centerValue ?? clickedValue;
            base.PrepareWithClickedValue(clickedValue);
        }

        protected override float Invoke(float value, int strength) => _centerValue ?? value;
    }
}