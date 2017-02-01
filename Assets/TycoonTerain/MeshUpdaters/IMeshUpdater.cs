using System;
using System.Collections.Generic;

public interface IMeshUpdater
{
    void UpdateMesh(MeshOptions options, IList<ITriangleMesh> terainMeshes);
}