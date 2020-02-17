using System;
using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.Utils;

namespace Votyra.Core.Painting.Commands
{
    public class FlattenLargeFactory : BaseFactory<FlattenLarge>
    {
        public FlattenLargeFactory(IEditableImage2f editableImage, IEditableMask2e editableMask, IThreadSafeLogger logger)
            : base(editableImage, editableMask, logger)
        {
        }

        public override string Action => KnownCommands.FlattenLarge;
    }

    public class FlattenLarge : PaintCommand
    {
        private const float smoothSpeedRelative = 0.2f;
        private float? _centerValue;

        public FlattenLarge()
            : base(2)
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

        protected override float Invoke(float value, int strength) => (MathUtils.Lerp(_centerValue ?? 0f, value, smoothSpeedRelative) - value) * Math.Sign(strength) + value;
    }
}