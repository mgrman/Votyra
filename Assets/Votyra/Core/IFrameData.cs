using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core
{
    public interface IPoolableFrameData : IFrameData
    {
        new Ray3f CameraRay { get; set; }

        new Plane3f[] CameraPlanes { get; }
        new Vector3f[] CameraFrustumCorners { get; }
    }

    public interface IFrameData
    {
        Ray3f CameraRay { get; }
        IReadOnlyList<Plane3f> CameraPlanes { get; }
        IReadOnlyList<Vector3f> CameraFrustumCorners { get; }
    }
}