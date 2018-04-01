using System;
using Votyra.Core.Images;
using Votyra.Core.Models;

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