using Votyra.Core.Images;
using Votyra.Core.InputHandling;
using Votyra.Core.Logging;
using Votyra.Core.Models;

namespace Votyra.Core.Painting.Commands
{
    public class FlattenFactory : IPaintCommandFactory
    {
        private readonly IEditableImage2f _editableImage;
        private readonly IEditableMask2e _editableMask;
        private readonly IThreadSafeLogger _logger;

        public FlattenFactory(IEditableImage2f editableImage, IEditableMask2e editableMask, IThreadSafeLogger logger)
        {
            _editableImage = editableImage;
            _editableMask = editableMask;
            _logger = logger;
        }

        public string Action => KnownCommands.Flatten;
        public IPaintCommand Create() => new Flatten(_editableImage, _editableMask,_logger);
    }
}