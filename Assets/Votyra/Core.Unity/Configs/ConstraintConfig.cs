namespace Votyra.Core.Images.Constraints
{
    public class ConstraintConfig : IConstraintConfig
    {
        public ConstraintConfig([ConfigInject("scaleFactor"),]
            int scaleFactor)
        {
            this.ScaleFactor = scaleFactor;
        }

        public int ScaleFactor { get; }
    }
}
