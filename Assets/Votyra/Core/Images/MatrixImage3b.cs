using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class MatrixImage3b : BaseMatrix3<bool>, IImage3b
    {
        public MatrixImage3b(Vector3i size)
            : base(size)
        {
        }

        public void UpdateImage(bool[,,] template)
        {
            base.UpdateImage(template);
        }

        public bool AnyData(Range3i range)
        {
            var allFalse = true;
            var allTrue = true;
            range = range.IntersectWith(_image.Range());
            var min = range.Min;
            for (var ix = 0; ix < range.Size.X; ix++)
            {
                for (var iy = 0; iy < range.Size.Y; iy++)
                {
                    for (var iz = 0; iz < range.Size.Z; iz++)
                    {
                        var value = _image[ix + min.X, iy + min.Y, iz + min.Z];
                        allFalse = allFalse && !value;
                        allTrue = allTrue && value;
                    }
                }
            }

            return !allFalse && !allTrue;
        }

        public Range3i InvalidatedArea { get; }
    }
}