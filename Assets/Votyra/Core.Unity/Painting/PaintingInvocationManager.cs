using UniRx;
using UnityEngine;
using Votyra.Core.Models;
using Zenject;

namespace Votyra.Core.Painting
{
    public class PaintingInvocationManager
    {
        protected IPaintingModel _paintingModel;

        [Inject]
        public PaintingInvocationManager(IPaintingModel paintingModel)
        {
            _paintingModel = paintingModel;

            _paintingModel.SelectedPaintCommand.PairWithPrevious()
                .Subscribe(o =>
                {
                    o.OldValue?.Unselected();
                    o.NewValue?.Selected();
                });

            _paintingModel.SelectedPaintCommand.CombineLatest(_paintingModel.PaintInvocationData, (cmd, data) => new {cmd, data})
                .Subscribe(o =>
                {
                    if (o.cmd != null && o.data != null)
                    {
                        Debug.Log($"Starting invocation of {o.cmd.GetType().Name} at {o.data.Value.ImagePosition} {o.data.Value.Strength}");
                        o.cmd.UpdateInvocationValues(o.data.Value.ImagePosition, o.data.Value.Strength);
                    }
                    else if (o.cmd != null)
                    {
                        Debug.Log($"Stoping invocation of {o.cmd.GetType().Name}");
                        o.cmd.StopInvocation();
                    }
                });
        }
    }
}