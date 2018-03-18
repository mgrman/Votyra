using System.Collections.Generic;
using Votyra.Core;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.GroupSelectors
{
    public interface IGroupVisibilityContext3i : IContext
    {
        Vector3f CameraPosition { get; }
        IEnumerable<Plane3f> CameraPlanes { get; }
        IEnumerable<Vector3f> CameraFrustumCorners { get; }
        Matrix4x4f CameraLocalToWorldMatrix { get; }
        Matrix4x4f ParentContainerWorldToLocalMatrix { get; }
        Rect3i InvalidatedArea_worldSpace { get; }
        IReadOnlySet<Vector3i> ExistingGroups { get; }
        Vector3i CellInGroupCount { get; }
    }
}