using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IFrameDataProvider3b
    {
        IFrameData3b GetCurrentFrameData(IReadOnlySet<Vector3i> existingGroups);
    }
}