using System;



using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Votyra.Core;
using Votyra.Core.Pooling;
using Votyra.Core.Models;

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
