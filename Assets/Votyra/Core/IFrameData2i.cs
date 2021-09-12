using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IFrameData2i : IFrameData
    {
        IImage2f Image { get; }
        Area1f RangeZ { get; }
        Range2i InvalidatedArea { get; }
    }
}