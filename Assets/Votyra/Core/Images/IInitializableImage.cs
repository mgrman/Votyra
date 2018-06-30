namespace Votyra.Core.Images
{
    public interface IInitializableImage
    {
        void FinishUsing();

        void StartUsing();
    }
}