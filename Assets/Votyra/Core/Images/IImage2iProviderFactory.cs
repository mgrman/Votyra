namespace Votyra.Core.Images
{
    public interface IImage2iProviderFactory
    {
        IImage2iProvider Create(IInitialImageConfig config);
    }
}