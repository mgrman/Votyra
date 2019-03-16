using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core
{
    public interface IPoolableFrameData2i : IFrameData2i, IPoolableFrameData, IPoolable<IPoolableFrameData2i>
    {
        IImage2f Image { get; set; }
        IMask2e Mask { get; set; }
        Range2i InvalidatedArea { get; set; }
        Vector2i CellInGroupCount { get; set; }
        int MeshSubdivision { get; set; }
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