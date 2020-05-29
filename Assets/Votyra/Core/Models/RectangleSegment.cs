using System;

namespace Votyra.Core.Models
{
    [Flags]
    public enum RectangleSegment : byte
    {
        None = 0,
        X0 = 1 << 0,
        X1 = 1 << 1,
        Y0 = 1 << 2,
        Y1 = 1 << 3,
        X0Y0 = X0 | Y0,
        X0Y1 = X0 | Y1,
        X1Y0 = X1 | Y0,
        X1Y1 = X1 | Y1,
    }

    public static class RectangleSideExtensions
    {
        public static bool IsInSegment(this RectangleSegment rectangleSegment, RectangleSegment flagToCheck) => (rectangleSegment & flagToCheck) != 0;
    }
}
