using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IFrameData2i : IFrameData
    {
        IImage2f Image { get; }
        IMask2e Mask { get; }
        Area1f RangeZ { get; }
        Range2i InvalidatedArea { get; }
        Vector2i CellInGroupCount { get; }
        int MeshSubdivision { get; }
    }
}