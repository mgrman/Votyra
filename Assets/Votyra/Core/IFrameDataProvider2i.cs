using System;
using Votyra.Core.Pooling;

namespace Votyra.Core
{
    public interface IFrameDataProvider2I
    {
        event Action<ArcResource<IFrameData2I>> FrameData;
    }
}
