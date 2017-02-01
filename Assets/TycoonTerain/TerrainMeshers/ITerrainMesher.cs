using System.Collections.Generic;

public interface ITerrainMesher
{
    void Initialize(TerrainOptions terrainOptions);
    void InitializeGroup( Vector2i group, ITriangleMesh mesh);
    int TriangleCount { get; }
    void AddCell(HeightData heightData, IMatrix<HeightData> data, Vector2i cellInGroup);
}