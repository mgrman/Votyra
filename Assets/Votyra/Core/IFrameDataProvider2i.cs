namespace Votyra.Core
{
    public interface IFrameDataProvider2i
    {
        IFrameData2i GetCurrentFrameData(int meshTopologyDistance, bool computedOnce);
    }
}