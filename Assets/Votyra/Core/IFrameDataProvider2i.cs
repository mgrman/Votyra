using System;

namespace Votyra.Core
{
    public interface IFrameDataProvider2i
    {
        event Action<IFrameData2i> FrameData;
    }
}