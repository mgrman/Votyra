using Votyra.Core.Models;

namespace Votyra.Core.Raycasting
{
    public interface IRaycasterAggregator
    {
        void Attach(IRaycaster raycaster);
    }
}