namespace Votyra.Core
{
    public interface IInterpolationConfig : IConfig
    {
        int ImageSubdivision { get; }
        int MeshSubdivision { get; }
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