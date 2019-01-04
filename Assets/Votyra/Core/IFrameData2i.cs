using System;
using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IFrameData2i : IFrameData, IDisposable
    {
        Vector2i CellInGroupCount { get; }
        IImage2f Image { get; }
        IMask2e Mask { get; }
        Range1hf RangeZ { get; }
        IReadOnlySet<Vector2i> ExistingGroups { get; }
        Range2i InvalidatedArea { get; }
        HashSet<Vector2i> SkippedAreas { get; }
    }
}