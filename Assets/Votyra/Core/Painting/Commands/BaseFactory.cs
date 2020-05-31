using Votyra.Core.Images;
using Votyra.Core.Logging;

namespace Votyra.Core.Painting.Commands
{
    public abstract class BaseFactory<T> : IPaintCommandFactory
        where T : IInitializablePaintCommand, new()
    {
        private readonly IEditableImage2f editableImage;
        private readonly IThreadSafeLogger logger;

        protected BaseFactory(IEditableImage2f editableImage, IThreadSafeLogger logger)
        {
            this.editableImage = editableImage;
            this.logger = logger;
        }

        public abstract string Action { get; }

        public IPaintCommand Create()
        {
            var cmd = new T();
            cmd.Initialize(this.editableImage, this.logger);
            return cmd;
        }
    }
}
