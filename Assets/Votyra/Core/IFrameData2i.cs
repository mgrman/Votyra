using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core
{
    public interface IPoolableFrameData2i : IFrameData2i, IPoolableFrameData
    {
        new IImage2f Image { get; set; }
        new IMask2e Mask { get; set; }
        new Range2i InvalidatedArea { get; set; }
        new Vector2i CellInGroupCount { get; set; }
        new int MeshSubdivision { get; set; }
    }
    
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