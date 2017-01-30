using System.Collections.Generic;

public interface ITerainMesher
{
    void Initialize(TerainOptions terainOptions, Vector2i group);
    void AddCell(HeightData heightData, IMatrix<HeightData> data, Vector2i cellInGroup);
    ITriangleMesh Result { get; }
}