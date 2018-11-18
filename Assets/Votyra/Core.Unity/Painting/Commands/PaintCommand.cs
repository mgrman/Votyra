using System;
using System.Threading;
using System.Threading.Tasks;
using UniRx.Async;
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

        protected virtual int PeriodMs { get; } = 100;

        [InjectOptional]
        protected IEditableMask2e _editableMask;

        [Inject]
        protected readonly IEditableImage2i _editableImage;

        private readonly object _invokeLock = new object();

        public virtual void StartInvocation(Vector2i cell, int maxStrength)
        {
            lock (_invokeLock)
            {
                if (_lastInvocation.IsCompleted)
                {
                    OnInvocationStarting();
                }
                var newInvocationCts = new CancellationTokenSource();
                _lastInvocationCts.Cancel();
                _lastInvocationCts = newInvocationCts;
                _lastInvocation = CombineAsync(_lastInvocation, cell, maxStrength, newInvocationCts.Token);
            }
        }


        private async Task CombineAsync(Task lastInvocation, Vector2i cell, int maxStrength, CancellationToken cancellationToken)
        {
            await lastInvocation;

            _maxStrength = maxStrength;
            _cell = cell;
            OnNewInvocationData();

            await InvokeRepeatAsync(cancellationToken);
        }

        protected async Task InvokeRepeatAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Invoke(cancellationToken);
                await UniTask.Delay(PeriodMs);
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

        public void StopInvocation()
        {
            OnInvocationStopping();
            lock (_invokeLock)
            {
                _lastInvocationCts.Cancel();
            }
        }

        protected virtual void OnNewInvocationData()
        {
        }

        protected virtual void OnInvocationStarting()
        {
        }

        protected virtual void OnInvocationStopping()
        {
        }

        protected virtual void PrepareWithClickedValue(Height clickedValue)
        {
        }

        protected virtual Height Invoke(Height value, int localStrength) => value;

        protected virtual MaskValues Invoke(MaskValues value, int localStrength) => value;
    }
}