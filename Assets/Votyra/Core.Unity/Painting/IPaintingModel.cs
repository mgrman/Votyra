using System.Collections.Generic;
using UniRx;
using Votyra.Core.Painting.Commands;

namespace Votyra.Core.Painting
{
    public interface IPaintingModel
    {
        IBehaviorSubject<IReadOnlyList<IPaintCommand>> PaintCommands { get; }
        IBehaviorSubject<IPaintCommand> SelectedPaintCommand { get; }
        IBehaviorSubject<PaintInvocationData?> PaintInvocationData { get; }
    }
}