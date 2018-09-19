using System;
using UniRx;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Painting.Commands;
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

            _paintingModel.SelectedPaintCommand
                .PairWithPrevious()
                .Subscribe(o =>
                {
                    o.OldValue?.Unselected();
                    o.NewValue?.Selected();
                });

            var activePaintCommand = _paintingModel.SelectedPaintCommand
                .CombineLatest(_paintingModel.Active, (cmd, active) => active ? cmd : null)
                .MakeLogExceptions();

            activePaintCommand
                .CombineLatest(_paintingModel.Strength, _paintingModel.ImagePosition, (cmd, strength, imagePosition) => new { cmd, strength, imagePosition })
                .Subscribe(o =>
                {
                    o.cmd?.Invoke(o.imagePosition, o.strength);
                });
        }
    }
}