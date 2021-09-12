namespace Votyra.Core.Unity.Config
{
    public class DualSampleConfig : IDualSampleConfig
    {
        public float WallSquishFactor { get; }

        public DualSampleConfig([ConfigInject("wallSquishFactor")] float wallSquishFactor)
        {
            WallSquishFactor = wallSquishFactor;
        }
    }
}