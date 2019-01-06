using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IFrameDataProvider2i
    {
        IFrameData2i GetCurrentFrameData(int meshTopologyDistance);
    }
}