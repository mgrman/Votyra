using System;
using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core
{
    public interface IFrameData : IDisposable
    {
        Vector3f CameraPosition { get; }
        IEnumerable<Plane3f> CameraPlanes { get; }
        IEnumerable<Vector3f> CameraFrustumCorners { get; }
        Matrix4x4f CameraLocalToWorldMatrix { get; }
        Matrix4x4f ParentContainerWorldToLocalMatrix { get; }
    }
}