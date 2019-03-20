using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IFrameData3b : IFrameData
    {
        IReadOnlySet<Vector3i> ExistingGroups { get; }
        IImage3b Image { get; }
        Range3i InvalidatedArea_imageSpace { get; }

        void Activate();

        void Deactivate();
    }
}