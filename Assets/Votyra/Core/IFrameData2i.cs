using System;
using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IFrameData2i : IFrameData, IDisposable
    {
        Range1f RangeZ { get; }
        IReadOnlySet<Vector2i> ExistingGroups { get; }
        IImage2f Image { get; }
        Range2i InvalidatedArea_imageSpace { get; }
    }
}