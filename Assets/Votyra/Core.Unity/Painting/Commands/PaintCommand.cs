using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Zenject;

namespace Votyra.Core.Painting.Commands
{
    public abstract class PaintCommand : IPaintCommand
    {
        protected int _maxStrength;
        protected Vector2i _cell;
        protected Task _lastInvocation = Task.CompletedTask;
        protected CancellationTokenSource _lastInvocationCts = new CancellationTokenSource();

        protected virtual float Period { get; } = 0.1f;

        [InjectOptional]
        protected IEditableMask2e _editableMask;

        [Inject]
        protected readonly IEditableImage2i _editableImage;

        private readonly object _invokeLock = new object();

        public virtual void Invoke(Vector2i cell, int maxStrength)
        {
            lock (_invokeLock)
            {
                _lastInvocationCts.Cancel();
                var invocationCts = new CancellationTokenSource();
                _lastInvocation = CombineAsync(_lastInvocation, () => InvokeRepeatAsync(invocationCts.Token));
                _lastInvocationCts = invocationCts;
            }

            _maxStrength = maxStrength;
            _cell = cell;
        }


        private async Task CombineAsync(Task a, Func<Task> b)
        {
            await a;
            InvokeReset();
            await b();
        }

        protected async Task InvokeRepeatAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Invoke(cancellationToken);
                await Task.Delay(TimeSpan.FromSeconds(Period));
            }
        }

        protected void Invoke(CancellationToken cancellationToken)
        {
            var absStrength = Math.Abs(_maxStrength);
            var absExtents = absStrength - 1;
            var cell = _cell;

            using (var image = _editableImage.RequestAccess(Range2i.FromCenterAndExtents(cell, new Vector2i(absStrength + 2, absStrength + 2))))
            {
                using (var mask = _editableMask?.RequestAccess(Range2i.FromCenterAndExtents(cell, new Vector2i(absStrength + 2, absStrength + 2))))
                {
                    PrepareWithClickedValue(image[cell]);
                    for (int ox = -absExtents; ox <= absExtents; ox++)
                    {
                        for (int oy = -absExtents; oy <= absExtents; oy++)
                        {
                            var index = cell + new Vector2i(ox, oy);
                            var cellStrength = (absStrength - Mathf.Max(Mathf.Abs(ox), Mathf.Abs(oy))) * Math.Sign(_maxStrength);
                            image[index] = Invoke(image[index], cellStrength);
                            mask[index] = Invoke(mask[index], cellStrength);

                            if (cancellationToken.IsCancellationRequested)
                                return;
                        }
                    }
                }
            }
        }

        public virtual void Selected()
        {
        }

        public virtual void Unselected()
        {
        }

        public virtual void InvokeReset()
        {
            lock (_invokeLock)
            {
                _lastInvocationCts.Cancel();
            }
        }

        protected virtual void PrepareWithClickedValue(Height clickedValue)
        {
        }

        protected virtual Height Invoke(Height value, int localStrength) => value;

        protected virtual MaskValues Invoke(MaskValues value, int localStrength) => value;
    }
}