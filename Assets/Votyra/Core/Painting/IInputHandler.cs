using Votyra.Core.InputHandling;
using Votyra.Core.Models;

namespace Votyra.Core.Painting
{
    public interface IInputHandler
    {
        bool Update(Ray3f inputRay, InputActions inputActions);
    }
}