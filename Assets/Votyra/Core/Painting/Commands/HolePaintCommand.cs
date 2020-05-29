using System;
using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.Painting.Commands
{
    public abstract class HolePaintCommand : IInitializablePaintCommand
    {
        private static readonly TimeSpan ClickDelay = TimeSpan.FromSeconds(0.2);
        private readonly int _maxDistance;

        private DateTime _clickLimit;
        private IEditableImage2f _editableImage;

        private IThreadSafeLogger _logger;

        protected HolePaintCommand(int maxDistance)
        {
            this._maxDistance = maxDistance;
        }

        private Vector2i? _lastInvocation { get; set; }

        public void Initialize(IEditableImage2f editableImage, IThreadSafeLogger logger)
        {
            this._editableImage = editableImage;
            this._logger = logger;
        }

        public virtual void UpdateInvocationValues(Vector2i cell)
        {
            var now = DateTime.UtcNow;
            if ((now < this._clickLimit) && ((this._lastInvocation == null) || ((this._lastInvocation.Value - cell).ManhattanMagnitude() < 3)))
            {
                return;
            }

            if (this._lastInvocation == null)
            {
                this._clickLimit = now + ClickDelay;
            }

            Path2iUtils.InvokeOnPath(this._lastInvocation, cell, this.Invoke);

            this._lastInvocation = cell;
        }

        public void Dispose()
        {
            this.StopInvocation();
        }

        public void StopInvocation()
        {
            this.OnInvocationStopping();
            this._lastInvocation = null;
            this._clickLimit = DateTime.MinValue;
        }

        protected void Invoke(Vector2i cell)
        {
            this.OnNewInvocationData();

            this._logger.LogMessage($"invoke on {cell}");

            var requestedArea = Range2i.FromMinAndMax(cell - this._maxDistance, cell + this._maxDistance + 1);
            using (var image = this._editableImage.RequestAccess(requestedArea))
            {
                var givenArea = image.Area;
                if (!givenArea.Contains(cell))
                {
                    return;
                }

                this.PrepareWithClickedValue(image[cell]);
                var min = givenArea.Min;
                var max = givenArea.Max;
                for (var ix = min.X; ix < max.X; ix++)
                {
                    for (var iy = min.Y; iy < max.Y; iy++)
                    {
                        var index = new Vector2i(ix, iy);
                        var ox = index.X - cell.X;
                        var oy = index.Y - cell.Y;

                        var cellStrength = Math.Max(Math.Abs(ox), Math.Abs(oy));
                        image[index] = this.Invoke(image[index], cellStrength);
                    }
                }
            }
        }

        protected virtual void OnNewInvocationData()
        {
        }

        protected virtual void OnInvocationStopping()
        {
        }

        protected virtual void PrepareWithClickedValue(float clickedValue)
        {
        }

        protected virtual float Invoke(float value, int localStrength) => value;
    }
}
