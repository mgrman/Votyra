using System.Collections.Generic;
using UnityEngine;
using Votyra.Core;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Cubical.GroupSelectors
{
    public interface IGroupVisibilityContext3i : IContext
    {
        Vector3 CameraPosition { get; }
        IReadOnlyList<Plane> CameraPlanes { get; }
        IReadOnlyPooledList<Vector3> CameraFrustumCorners { get; }
        Matrix4x4 CameraLocalToWorldMatrix { get; }
        Matrix4x4 ParentContainerWorldToLocalMatrix { get; }
        Rect3i? InvalidatedArea_worldSpace { get; }
        IReadOnlySet<Vector3i> ExistingGroups { get; }
        Vector3i CellInGroupCount { get; }
    }
}