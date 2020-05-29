namespace Votyra.Core.Raycasting
{
    public interface IRaycasterAggregator : IRaycaster
    {
        void Attach(IRaycasterPart raycaster);
    }
}
