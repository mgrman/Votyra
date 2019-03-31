using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Logging;
using Votyra.Core.Painting.Commands;
using Votyra.Core.Utils;

namespace Votyra.Core.Painting
{
    public class PaintingModel : IPaintingModel, IDisposable
    {
        private readonly IThreadSafeLogger _logger;
        private IPaintCommand _selectedPaintCommand;

        public PaintingModel(List<IPaintCommand> commands, IThreadSafeLogger logger)
        {
            _logger = logger;
            PaintCommands = commands;

            SelectedPaintCommand = commands.OfType<IncreaseOrDecrease>()
                .FirstOrDefault();
        }

        public void Dispose()
        {
            SelectedPaintCommand.TryDispose();
        }

        public IReadOnlyList<IPaintCommand> PaintCommands { get; }

        public IPaintCommand SelectedPaintCommand { get; set; }

        public bool IsExtendedModifierActive { get; set; }
        public bool IsInvertModifierActive { get; set; }
    }
}