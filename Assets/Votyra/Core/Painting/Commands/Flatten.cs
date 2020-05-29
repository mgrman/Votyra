using Votyra.Core.Images;
using Votyra.Core.Logging;

namespace Votyra.Core.Painting.Commands
{
    public class FlattenFactory : BaseFactory<Flatten>
    {
        public FlattenFactory(IEditableImage2f editableImage, IThreadSafeLogger logger)
            : base(editableImage, logger)
        {
        }

        public override string Action => KnownCommands.Flatten;
    }

    public class Flatten : PaintCommand
    {
        private float? _centerValue;

        public Flatten()
            : base(0)
        {
        }

        protected override void OnInvocationStopping()
        {
            base.OnInvocationStopping();
            this._centerValue = null;
        }

        protected override void PrepareWithClickedValue(float clickedValue)
        {
            this._centerValue = this._centerValue ?? clickedValue;
            base.PrepareWithClickedValue(clickedValue);
        }

        protected override float Invoke(float value, int strength) => this._centerValue ?? value;
    }
}
