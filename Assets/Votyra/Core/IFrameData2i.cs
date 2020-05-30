using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IPoolableFrameData2I : IFrameData2I, IPoolableFrameData
    {
        new IImage2F Image { get; set; }

        new Range2i InvalidatedArea { get; set; }
    }

    public interface IFrameData2I : IFrameData
    {
        IImage2F Image { get; }

        Area1f RangeZ { get; }

        Range2i InvalidatedArea { get; }
    }
}
