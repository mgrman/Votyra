using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IInterpolationConfig : ISharedConfig
    {
        int ImageSubdivision { get; }

        Vector2i MeshSubdivision { get; }

        bool DynamicMeshes { get; }

        IntepolationAlgorithm ActiveAlgorithm { get; }
    }

    public enum IntepolationAlgorithm
    {
        NearestNeighbour,
        Linear,
        Cubic
    }
}
