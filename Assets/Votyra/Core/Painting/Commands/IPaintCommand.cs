using System;
using Votyra.Core.Models;

namespace Votyra.Core.Painting.Commands
{
    public interface IPaintCommand : IDisposable
    {
        void UpdateInvocationValues(Vector2i cell, int strength);
    }
}