using System;
using UnityEngine;
using Votyra.Core.Pooling;

namespace Votyra.Core.Unity
{
    public interface ITerrainGameObject
    {
        void InitializeOnMainThread();
        GameObject GameObject { get; }
        MeshFilter MeshFilter { get; }
        MeshCollider MeshCollider { get; }
        BoxCollider BoxCollider { get; }
    }
}