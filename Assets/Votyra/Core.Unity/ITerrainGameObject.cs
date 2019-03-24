using System;
using System.Threading.Tasks;
using UnityEngine;
using Votyra.Core.Pooling;

namespace Votyra.Core.Unity
{
    public interface ITerrainGameObject
    {
        bool IsInitialized { get; }
        void Initialize();
        Task InitializeAsync();
        void SetActive(bool isActive);
        Task SetActiveAsync(bool isActive);
        GameObject GameObject { get; }
        MeshFilter MeshFilter { get; }
        MeshCollider MeshCollider { get; }
        BoxCollider BoxCollider { get; }
    }
}