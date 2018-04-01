using System;
using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core
{
    public interface IFrameDataProvider<TFrameData>
    where TFrameData : IFrameData
    {
        TFrameData GetCurrentFrameData();
    }
}