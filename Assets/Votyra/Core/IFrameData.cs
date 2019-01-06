using System;
using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core
{
    public interface IFrameData 
    {
        Vector3f CameraPosition { get; }
        IReadOnlyPooledList<Plane3f> CameraPlanes { get; }
        IReadOnlyPooledList<Vector3f> CameraFrustumCorners { get; }
        Matrix4x4f CameraLocalToWorldMatrix { get; }
        Matrix4x4f ParentContainerWorldToLocalMatrix { get; }

        void Activate();

        void Deactivate();
        
    }
}