using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MeshGenerationResult
{
    public readonly ITriangleMesh Mesh;
    public readonly Vector2i Group;

    public MeshGenerationResult(ITriangleMesh mesh, Vector2i group)
    {
        this.Mesh = mesh;
        this.Group = group;
    }
}
