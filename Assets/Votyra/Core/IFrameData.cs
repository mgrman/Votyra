using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IFrameData
    {
        Ray3f CameraRay { get; }
        IReadOnlyList<Plane3f> CameraPlanes { get; }
        IReadOnlyList<Vector3f> CameraFrustumCorners { get; }

        void Activate();

        void Deactivate();
    }
}