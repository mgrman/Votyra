using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public interface IGeneralMesh : IMesh
    {
        void Reset(Area3f area);

        void AddTriangle(Vector3f a, Vector3f b, Vector3f c);

        void FinalizeMesh();

        IEnumerable<Triangle3f> GetTriangles(Vector2i? limitToCellInGroup);
    }
}
