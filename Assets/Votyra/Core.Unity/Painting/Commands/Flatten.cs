using System;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Zenject;

namespace Votyra.Core.Painting.Commands
{
    public class Flatten : PaintCommand
    {
        private const float smoothSpeedRelative = 0.2f;
        private Height1i? _centerValue;

        protected override void OnInvocationStopping()
        {
            base.OnInvocationStopping();
            _centerValue = null;
        }

        protected override void PrepareWithClickedValue(Height1i clickedValue)
        {
            _centerValue = _centerValue ?? clickedValue;
            base.PrepareWithClickedValue(clickedValue);
        }

        protected override Height1i Invoke(Height1i value, int strength)
        {
            return (Height1i.Lerp(_centerValue ?? Height1i.Default, value, smoothSpeedRelative) - value) * Math.Sign(strength) + value;
        }
    }
}