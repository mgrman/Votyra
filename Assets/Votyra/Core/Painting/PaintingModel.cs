using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Logging;
using Votyra.Core.Painting.Commands;
using Votyra.Core.Utils;

namespace Votyra.Core.Painting
{
    public class PaintingModel : IPaintingModel
    {
        private readonly IThreadSafeLogger _logger;
        private IPaintCommand _selectedPaintCommand;

        public PaintingModel(List<IPaintCommandFactory> paintCommandFactories, IThreadSafeLogger logger)
        {
            _logger = logger;
            PaintCommandFactories = paintCommandFactories;
        }
        public IReadOnlyList<IPaintCommandFactory> PaintCommandFactories { get; }
    }
}