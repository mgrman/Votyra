using System;



using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Pooling;
using Votyra.Core;
using Votyra.Core.Models;

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
