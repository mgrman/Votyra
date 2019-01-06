using System;
using System.Collections.Generic;
using UniRx;
using Votyra.Core.Models;
using Votyra.Core.Painting.Commands;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Painting
{
    public class PaintingModel : IPaintingModel, IDisposable
    {
        [Inject]
        public PaintingModel([InjectOptional] List<IPaintCommand> commands)
        {
            PaintCommands.OnNext(commands);
        }

        public void Dispose()
        {
            SelectedPaintCommand.TryDispose();
            PaintInvocationData.TryDispose();
        }

        public IBehaviorSubject<IReadOnlyList<IPaintCommand>> PaintCommands { get; } = new BehaviorSubject<IReadOnlyList<IPaintCommand>>(Array.Empty<IPaintCommand>()).MakeDistinct()
            .MakeScheduledOnMainThread()
            .MakeLogExceptions();

        public IBehaviorSubject<IPaintCommand> SelectedPaintCommand { get; } = new BehaviorSubject<IPaintCommand>(null).MakeDistinct()
            .MakeScheduledOnMainThread()
            .MakeLogExceptions();

        public IBehaviorSubject<PaintInvocationData?> PaintInvocationData { get; } = new BehaviorSubject<PaintInvocationData?>(null).MakeDistinct()
            .MakeScheduledOnMainThread()
            .MakeLogExceptions();
    }
}