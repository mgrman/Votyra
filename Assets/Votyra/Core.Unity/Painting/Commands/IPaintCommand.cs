using Votyra.Core.Models;

namespace Votyra.Core.Painting.Commands
{
    public interface IPaintCommand
    {
        void Selected();

        void Unselected();

        void StopInvocation();

        void UpdateInvocationValues(Vector2i cell, int strength);
    }
}