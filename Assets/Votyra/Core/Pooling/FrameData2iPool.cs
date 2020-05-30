namespace Votyra.Core.Pooling
{
    public class FrameData2IPool : ArcPool<IFrameData2I>, IFrameData2IPool
    {
        public FrameData2IPool()
            : base(Create)
        {
        }

        private static IFrameData2I Create() => new FrameData2I();
    }
}
