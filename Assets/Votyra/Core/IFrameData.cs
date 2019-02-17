using Votyra.Core.InputHandling;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core
{
    public interface IFrameData
    {
        Ray3f CameraRay { get; }
        IReadOnlyPooledList<Plane3f> CameraPlanes { get; }
        IReadOnlyPooledList<Vector3f> CameraFrustumCorners { get; }

        void Activate();

        void Deactivate();
    }
}