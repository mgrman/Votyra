using System.Collections;
using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.Images.Constraints;

namespace Votyra.Core.Images
{
    public interface IImageConfig
    {
        Vector3i ImageSize { get; }
    }
}