using Votyra.Core.InputHandling;
using Votyra.Core.Models;

namespace Votyra.Core.Painting
{
    public class InputData
    {
        public InputData(Ray3f inputRay, InputActions inputActions)
        {
            InputRay = inputRay;
            InputActions = inputActions;
        }

        public Ray3f InputRay { get; }
        public InputActions InputActions { get; }
    }
}