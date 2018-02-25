using System.Collections.Generic;
using UnityEngine;
using Votyra.Core;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Plannar.GroupSelectors
{
    public interface IGroupVisibilityContext2i : IContext
    {
        Vector3 CameraPosition { get; }
        IReadOnlyList<Plane> CameraPlanes { get; }
        IReadOnlyPooledList<Vector3> CameraFrustumCorners { get; }
        Matrix4x4 CameraLocalToWorldMatrix { get; }
        Matrix4x4 ParentContainerWorldToLocalMatrix { get; }
        Bounds GroupBounds { get; }
        Rect2i InvalidatedArea_worldSpace { get; }
        IReadOnlySet<Vector2i> ExistingGroups { get; }
        Vector2i CellInGroupCount { get; }
    }
}