using Votyra.Core.Models;

namespace Votyra.Core.TerrainMeshes
{
    public interface ITerrainMesh2F : IMesh
    {
        void Reset(Area3f area);

        void AddCell(Vector2i cellInGroup, Vector2i subCell, SampledData2F data);

        void FinalizeMesh();

        float Raycast(Vector2f point);

        Vector3f Raycast(Ray3f ray);
    }
}
