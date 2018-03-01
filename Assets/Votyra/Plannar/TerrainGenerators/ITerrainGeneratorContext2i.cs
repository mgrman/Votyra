using Votyra.Core;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Plannar.ImageSamplers;

namespace Votyra.Plannar.TerrainGenerators
{
    public interface ITerrainGeneratorContext2i : IContext
    {
        Vector2i CellInGroupCount { get; }
        Rect3f GroupBounds { get; }
        IImageSampler2i ImageSampler { get; }
        IImage2f Image { get; }
    }
}