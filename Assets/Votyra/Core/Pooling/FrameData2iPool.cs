namespace Votyra.Core.Pooling
{
    public class FrameData2iPool : ArcPool<IFrameData2i>, IFrameData2iPool
    {
        public FrameData2iPool()
            : base(Create)
        {
        }

        private static IFrameData2i Create() => new FrameData2i();
    }
}
