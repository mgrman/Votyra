using Votyra.Core;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.ImageSamplers;

namespace Votyra.Core.TerrainGenerators
{
    public interface ITerrainGeneratorContext2i : IContext
    {
        Vector2i CellInGroupCount { get; }
        Rect3f GroupBounds { get; }
        IImageSampler2i ImageSampler { get; }
        IImage2f Image { get; }
    }
}