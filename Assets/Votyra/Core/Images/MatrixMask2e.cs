using System;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class MatrixMask2e : BaseMatrix2<MaskValues>, IMask2e
    {

        public MatrixMask2e(Vector2i size)
            : base(size)
        {
        }
    }
}