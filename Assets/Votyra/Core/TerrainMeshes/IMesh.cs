using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public interface IMesh
    {
        uint VertexCount { get; }

        uint TriangleCapacity { get; }

        uint TriangleCount { get; }

        IReadOnlyList<Vector3f> Vertices { get; }

        IReadOnlyList<Vector3f> Normals { get; }

        IReadOnlyList<Vector2f> UV { get; }

        IReadOnlyList<int> Indices { get; }

        Area3f MeshBounds { get; }
    }
}
