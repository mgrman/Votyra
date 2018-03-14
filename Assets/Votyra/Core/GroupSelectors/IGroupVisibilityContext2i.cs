using System.Collections.Generic;
using Votyra.Core;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.GroupSelectors
{
    public interface IGroupVisibilityContext2i : IContext
    {
        Vector3f CameraPosition { get; }
        IEnumerable<Plane3f> CameraPlanes { get; }
        IEnumerable<Vector3f> CameraFrustumCorners { get; }
        Matrix4x4f CameraLocalToWorldMatrix { get; }
        Matrix4x4f ParentContainerWorldToLocalMatrix { get; }
        Rect3f GroupBounds { get; }
        Rect2i InvalidatedArea_worldSpace { get; }
        IReadOnlySet<Vector2i> ExistingGroups { get; }
        Vector2i CellInGroupCount { get; }
    }
}