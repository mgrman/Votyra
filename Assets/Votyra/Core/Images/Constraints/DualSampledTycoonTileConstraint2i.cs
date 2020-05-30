using Votyra.Core.Logging;
using Votyra.Core.Models;

namespace Votyra.Core.Images.Constraints
{
    public class DualSampledTycoonTileConstraint2I : TycoonTileConstraint2I
    {
        public DualSampledTycoonTileConstraint2I(IConstraintConfig constraintConfig, IThreadSafeLogger logger)
            : base(constraintConfig, logger)
        {
        }

        protected override void ConstrainCell(Vector2i cell)
        {
            if ((cell % 2) == Vector2i.One)
            {
                base.ConstrainCell(cell);
            }
        }
    }
}
