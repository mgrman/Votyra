namespace Votyra.Core
{
    public interface IFrameDataProvider<TFrameData>
        where TFrameData : IFrameData
    {
        TFrameData GetCurrentFrameData();
    }
}
