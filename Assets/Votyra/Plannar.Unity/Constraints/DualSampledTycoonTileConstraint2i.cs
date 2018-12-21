using Votyra.Core;
using Votyra.Core.Images;
using Votyra.Core.Images.Constraints;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;

namespace Votyra.Plannar.Images.Constraints
{
    public class DualSampledTycoonTileConstraint2i : TycoonTileConstraint2i
    {
        public DualSampledTycoonTileConstraint2i(IImageSampler2i sampler, [ConfigInject("scaleFactor")] int scaleFactor)
        : base(sampler, scaleFactor)
        {
        }

        protected override void ConstrainCell(Matrix2<Height> editableMatrix, Vector2i cell)
        {
            if (cell % 2 == Vector2i.One)
            {
                base.ConstrainCell(editableMatrix, cell);
            }
        }
    }
}