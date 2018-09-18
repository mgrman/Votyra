using System;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Zenject;

namespace Votyra.Core.Painting.Commands
{
    public interface IPaintCommand
    {
        void Selected();

        void Unselected();

        void InvokeReset();

        void Invoke(Vector2i cell, int strength);
    }
}