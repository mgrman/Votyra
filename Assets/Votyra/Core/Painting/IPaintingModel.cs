using System;
using System.Collections.Generic;
using Votyra.Core.Painting.Commands;

namespace Votyra.Core.Painting
{
    public interface IPaintingModel
    {
        IReadOnlyList<IPaintCommand> PaintCommands { get; }

        IPaintCommand SelectedPaintCommand { get; set; }
        bool IsExtendedModifierActive { get; set; }
        bool IsInvertModifierActive { get; set; }

    }
}