using System;
using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.Utils;

namespace Votyra.Core.Painting.Commands
{
    public class Flatten : PaintCommand
    {
        private const float SmoothSpeedRelative = 0.2f;
        private float? _centerValue;

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

        protected override float Invoke(float value, int strength) => (MathUtils.Lerp(_centerValue ?? 0f, value, SmoothSpeedRelative) - value) * Math.Sign(strength) + value;

        public Flatten(IEditableImage2f editableImage, IThreadSafeLogger logger)
            : base(editableImage, logger)
        {
        }
    }
}