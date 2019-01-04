using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IInterpolationConfig : IConfig
    {
        int Subdivision { get; }

        IntepolationAlgorithm ActiveAlgorithm { get; }
    }

    public enum IntepolationAlgorithm
    {
        NearestNeighbour,
        Linear,
        Cubic   
    }
}