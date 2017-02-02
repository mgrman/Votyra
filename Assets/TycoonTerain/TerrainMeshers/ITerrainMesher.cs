using System.Collections.Generic;

public interface ITerrainMesher
{
    void Initialize(TerrainOptions terrainOptions);
    void InitializeGroup( Vector2i group, ITriangleMesh mesh, IMatrix<ResultHeightData> data);
    int TriangleCount { get; }
    void AddCell(Vector2i cellInGroup);
}