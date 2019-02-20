using Votyra.Core.Models;

namespace Votyra.Core.Raycasting
{
    public interface IRaycaster
    {
         Vector2f? Raycast(Ray3f cameraRay);
    }
}