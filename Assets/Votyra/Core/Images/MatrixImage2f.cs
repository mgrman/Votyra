using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class MatrixImage2F : BaseMatrix2<float>, IImage2F
    {
        public MatrixImage2F(Vector2i size)
            : base(size)
        {
        }

        public Area1f RangeZ { get; private set; }

        public void UpdateImage(float[,] template, Area1f rangeZ)

        {
            this.UpdateImage(template);

            this.RangeZ = rangeZ;
        }
    }
}