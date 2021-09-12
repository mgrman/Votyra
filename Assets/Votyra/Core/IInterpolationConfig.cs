namespace Votyra.Core
{
    public interface IInterpolationConfig : IConfig
    {
        int MeshSubdivision { get; }
        bool DynamicMeshes { get; }
        bool IsBicubic { get; }
    }
}