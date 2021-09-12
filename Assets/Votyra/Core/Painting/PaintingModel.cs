using System;
using System.Collections.Generic;
using Votyra.Core.Logging;
using Votyra.Core.Models;
using Votyra.Core.Painting.Commands;
using Votyra.Core.Utils;

namespace Votyra.Core.Painting
{
    public class PaintingModel : IPaintingModel, IDisposable
    {
        private IPaintCommand _selectedPaintCommand;

        public IReadOnlyList<IPaintCommand> PaintCommands { get; }

        public IPaintCommand SelectedPaintCommand { get; set; }

        public bool IsExtendedModifierActive { get; set; }
        
        public bool IsInvertModifierActive { get; set; }
        
        public PaintingModel(List<IPaintCommand> commands)
        {
            PaintCommands = commands;
        }

        public void Dispose()
        {
            SelectedPaintCommand.TryDispose();
        }

    }
}