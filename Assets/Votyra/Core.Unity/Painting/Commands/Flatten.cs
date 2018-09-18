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
        private Height? _centerValue;

        public override void InvokeReset()
        {
            base.InvokeReset();
            _centerValue = null;
        }

        protected override void PrepareWithClickedValue(Height clickedValue)
        {
            _centerValue = _centerValue ?? clickedValue;
            base.PrepareWithClickedValue(clickedValue);
        }

        protected override Height Invoke(Height value, int strength)
        {
            return (Height.Lerp(_centerValue ?? Height.Default, value, smoothSpeedRelative) - value) * Math.Sign(strength) + value;
        }
    }
}