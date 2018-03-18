using System.Collections;
using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.Images.Constraints;
using System;

namespace Votyra.Core
{
    public interface IImageConfig
    {
        Vector3i ImageSize { get; }
    }
}