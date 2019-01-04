using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IFrameDataProvider2i
    {
        IFrameData2i GetCurrentFrameData(IReadOnlySet<Vector2i> existingGroups);
    }
}