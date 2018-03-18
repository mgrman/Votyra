using System.Collections;
using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Core.Images.Constraints;
using System;

namespace Votyra.Core
{
    public interface IInitialImageConfig
    {
        object InitialData { get; }
        Vector3f InitialDataScale { get; }
    }
}