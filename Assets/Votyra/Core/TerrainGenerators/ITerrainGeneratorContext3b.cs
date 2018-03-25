using Votyra.Core;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.ImageSamplers;

namespace Votyra.Core.TerrainGenerators
{
    public interface ITerrainGeneratorContext3b : IContext
    {
        Vector3i CellInGroupCount { get; }
        IImageSampler3 ImageSampler { get; }
        IImage3b Image { get; }
    }
}