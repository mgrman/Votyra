using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class MatrixImage2f : BaseMatrix2<float>, IImage2f
    {
        public MatrixImage2f(Vector2i size)
            : base(size)
        {
        }

        public Area1f RangeZ { get; private set; }

        public void UpdateImage(Matrix2<float> template, Area1f rangeZ)

        {
            base.UpdateImage(template);

            RangeZ = rangeZ;
        }
    }
}