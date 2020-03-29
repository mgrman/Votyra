namespace Votyra.Core
{
    public class DualSampleConfig : IDualSampleConfig
    {
        public DualSampleConfig([ConfigInject("wallSquishFactor")] float wallSquishFactor)
        {
            WallSquishFactor = wallSquishFactor;
        }

        public float WallSquishFactor { get; }
    }
}
