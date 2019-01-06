using Votyra.Core;
using Votyra.Core.Models;

namespace Votyra.Plannar.Images.Constraints
{
    public class DualSampledTycoonTileConstraint2i : TycoonTileConstraint2i
    {
        public DualSampledTycoonTileConstraint2i([ConfigInject("scaleFactor")] int scaleFactor)
            : base(scaleFactor)
        {
        }

        protected override void ConstrainCell(Vector2i cell)
        {
            if (cell % 2 == Vector2i.One)
                base.ConstrainCell(cell);
        }
    }
}