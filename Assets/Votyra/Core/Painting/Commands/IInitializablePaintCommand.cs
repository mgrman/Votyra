using Votyra.Core.Images;
using Votyra.Core.Logging;

namespace Votyra.Core.Painting.Commands
{
    public interface IInitializablePaintCommand : IPaintCommand
    {
        void Initialize(IEditableImage2F editableImage, IThreadSafeLogger logger);
    }
}
