using System;
using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IFrameData2i : IFrameData, IDisposable
    {
        Range1h RangeZ { get; }
        IReadOnlySet<Vector2i> ExistingGroups { get; }
        IImage2i Image { get; }
        IMask2e Mask { get; }
        Range2i InvalidatedArea { get; }
    }
}