using System;
using UnityEngine;

namespace Votyra.Core.Painting.Commands
{
    public class Flatten : PaintCommand
    {
        private const float smoothSpeedRelative = 0.2f;
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

        protected override float Invoke(float value, int strength) => (Mathf.Lerp(_centerValue ?? 0f, value, smoothSpeedRelative) - value) * Math.Sign(strength) + value;
    }
}