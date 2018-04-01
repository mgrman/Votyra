using System;
using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core
{
    public interface IFrameData2i : IFrameData, IDisposable
    {
        Range2f RangeZ { get; }
        IReadOnlySet<Vector2i> ExistingGroups { get; }
        IImage2f Image { get; }
        Rect2i InvalidatedArea_imageSpace { get; }
    }
}