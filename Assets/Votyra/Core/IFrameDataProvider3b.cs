using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IFrameDataProvider3B
    {
        IFrameData3B GetCurrentFrameData(IReadOnlySet<Vector3i> existingGroups);
    }
}
