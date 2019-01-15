namespace Votyra.Core.Painting
{
    public class PaintingInvocationManager
    {
        private readonly IPaintingModel _paintingModel;

        public PaintingInvocationManager(IPaintingModel paintingModel)
        {
            _paintingModel = paintingModel;
        }
    }
}