using System;
using UnityEngine;
using Votyra.Core.Pooling;

namespace Votyra.Core.Unity
{
    public interface ITerrainGameObject : IPoolable<ITerrainGameObject>
    {
        GameObject GameObject { get; }
        MeshFilter MeshFilter { get; }
        MeshCollider MeshCollider { get; }
        BoxCollider BoxCollider { get; }
    }
}