using System;
using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core
{
    public interface IFrameData3b : IFrameData, IDisposable
    {
        IReadOnlySet<Vector3i> ExistingGroups { get; }
        IImage3b Image { get; }
        Rect3i InvalidatedArea_imageSpace { get; }
    }
}