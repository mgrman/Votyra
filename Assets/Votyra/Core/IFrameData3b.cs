using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface IFrameData3B : IFrameData
    {
        IReadOnlySet<Vector3i> ExistingGroups { get; }

        IImage3B Image { get; }

        Range3i InvalidatedAreaImageSpace { get; }

        void Activate();

        void Deactivate();
    }
}
