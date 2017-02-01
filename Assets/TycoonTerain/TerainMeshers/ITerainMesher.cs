using System.Collections.Generic;

public interface ITerainMesher
{
    void Initialize(TerainOptions terainOptions);
    void InitializeGroup( Vector2i group, ITriangleMesh mesh);
    int TriangleCount { get; }
    void AddCell(HeightData heightData, IMatrix<HeightData> data, Vector2i cellInGroup);
}