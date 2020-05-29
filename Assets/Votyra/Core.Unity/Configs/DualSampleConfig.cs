namespace Votyra.Core
{
    public class DualSampleConfig : IDualSampleConfig
    {
        public DualSampleConfig([ConfigInject("wallSquishFactor"),]
            float wallSquishFactor)
        {
            this.WallSquishFactor = wallSquishFactor;
        }

        public float WallSquishFactor { get; }
    }
}
