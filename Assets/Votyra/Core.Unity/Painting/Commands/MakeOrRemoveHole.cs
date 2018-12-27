using System;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Zenject;

namespace Votyra.Core.Painting.Commands
{
    public class MakeOrRemoveHole : PaintCommand
    {
        protected override MaskValues Invoke(MaskValues value, int strength)
        {
            return strength < 0f ? MaskValues.Hole : MaskValues.Terrain;
        }
    }
}