using System;
using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IFrameData 
    {
        Vector3f CameraPosition { get; }
        Plane3f[] CameraPlanes { get; }
        Vector3f[] CameraFrustumCorners { get; }
        Matrix4x4f CameraLocalToWorldMatrix { get; }
        Matrix4x4f ParentContainerWorldToLocalMatrix { get; }

        void Activate();

        void Deactivate();
        
    }
}