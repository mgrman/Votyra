namespace Votyra.Core.Images.Constraints
{
    public class ConstraintConfig : IConstraintConfig
    {
        public ConstraintConfig([ConfigInject("scaleFactor")] int scaleFactor)
        {
            ScaleFactor = scaleFactor;
        }

        public int ScaleFactor { get; }
    }
}
