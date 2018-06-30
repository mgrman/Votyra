using System;
using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IFrameData : IDisposable
    {
        IEnumerable<Vector3f> CameraFrustumCorners { get; }
        Matrix4x4f CameraLocalToWorldMatrix { get; }
        IEnumerable<Plane3f> CameraPlanes { get; }
        Vector3f CameraPosition { get; }
        Matrix4x4f ParentContainerWorldToLocalMatrix { get; }
    }
}