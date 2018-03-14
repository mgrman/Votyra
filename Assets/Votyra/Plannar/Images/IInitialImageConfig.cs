using System.Collections;
using System.Collections.Generic;
using Votyra.Core.Models;
using Votyra.Plannar.Images.Constraints;

namespace Votyra.Plannar.Images
{
    public interface IInitialImageConfig : IImageConfig
    {
        object InitialData { get; }
        Vector3f InitialDataScale { get; }
    }
}