using System;
using System.Collections.Generic;

public interface ITerainGeneratorApplicator:IDisposable
{
    void UpdateMesh(TerainGeneratorOptions options, IDictionary<Vector2i, ITriangleMesh> triangleMeshes);
}