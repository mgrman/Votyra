using System;
using Votyra.Core.Pooling;

namespace Votyra.Core
{
    public interface IFrameDataProvider2i
    {
        event Action<ArcResource<IFrameData2i>> FrameData;
    }
}