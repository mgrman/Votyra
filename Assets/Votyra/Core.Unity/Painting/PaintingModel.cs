using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using Votyra.Core.Models;
using Votyra.Core.Painting.Commands;
using Votyra.Core.Utils;

namespace Votyra.Core.Painting
{
    public class PaintingModel : IPaintingModel, IDisposable
    {
        public IBehaviorSubject<IReadOnlyList<IPaintCommand>> PaintCommands { get; } = new BehaviorSubject<IReadOnlyList<IPaintCommand>>(Array.Empty<IPaintCommand>())
            .MakeDistinct()
            .MakeScheduledOnMainThread()
            .MakeLogExceptions();

        public IBehaviorSubject<IPaintCommand> SelectedPaintCommand { get; } = new BehaviorSubject<IPaintCommand>(null)
            .MakeDistinct()
            .MakeScheduledOnMainThread()
            .MakeLogExceptions();

        public IBehaviorSubject<bool> Active { get; } = new BehaviorSubject<bool>(false)
            .MakeDistinct()
            .MakeScheduledOnMainThread()
            .MakeLogExceptions();

        public IBehaviorSubject<int> Strength { get; } = new BehaviorSubject<int>(0)
            .MakeDistinct()
            .MakeScheduledOnMainThread()
            .MakeLogExceptions();

        public IBehaviorSubject<Vector2i> ImagePosition { get; } = new BehaviorSubject<Vector2i>(Vector2i.Zero)
            .MakeDistinct()
            .MakeScheduledOnMainThread()
            .MakeLogExceptions();

        public void Dispose()
        {
            SelectedPaintCommand.TryDispose();
            Active.TryDispose();
            Strength.TryDispose();
        }
    }
}