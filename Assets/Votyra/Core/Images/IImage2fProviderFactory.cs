using Votyra.Core.Images;

namespace Votyra.Core.Images
{
    public interface IImage2fProviderFactory
    {
        IImage2fProvider Create(IInitialImageConfig config);
    }
}