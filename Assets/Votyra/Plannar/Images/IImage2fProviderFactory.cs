using Votyra.Core.Images;

namespace Votyra.Plannar.Images
{
    public interface IImage2fProviderFactory
    {
        IImage2fProvider Create(IInitialImageConfig config);
    }
}