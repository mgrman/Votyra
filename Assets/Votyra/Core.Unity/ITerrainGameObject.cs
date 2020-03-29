using System.Threading.Tasks;
using UnityEngine;

namespace Votyra.Core.Unity
{
    public interface ITerrainGameObject
    {
        bool IsInitialized { get; }

        GameObject GameObject { get; }

        MeshFilter MeshFilter { get; }

        MeshCollider MeshCollider { get; }

        BoxCollider BoxCollider { get; }

        void Initialize();

        Task InitializeAsync();

        void SetActive(bool isActive);

        Task SetActiveAsync(bool isActive);
    }
}
