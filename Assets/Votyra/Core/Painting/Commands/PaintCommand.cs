using System;
using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.Painting.Commands
{
    public abstract class PaintCommand : IInitializablePaintCommand
    {
        private static readonly TimeSpan ClickDelay = TimeSpan.FromSeconds(0.2);
        private readonly int _maxDistance;

        private DateTime _clickLimit;
        private IEditableImage2f _editableImage;

        private IThreadSafeLogger _logger;

        protected PaintCommand(int maxDistance)
        {
            _maxDistance = maxDistance;
        }

        private Vector2i? _lastInvocation { get; set; }

        public void Initialize(IEditableImage2f editableImage, IThreadSafeLogger logger)
        {
            _editableImage = editableImage;
            _logger = logger;
        }

        public virtual void UpdateInvocationValues(Vector2i cell)
        {
            var now = DateTime.UtcNow;
            if (now < _clickLimit && (_lastInvocation == null || (_lastInvocation.Value - cell).ManhattanMagnitude() < 3))
            {
                return;
            }

            if (_lastInvocation == null)
            {
                _clickLimit = now + ClickDelay;
            }

            Path2iUtils.InvokeOnPath(_lastInvocation, cell, Invoke);

            _lastInvocation = cell;
        }

        public void Dispose()
        {
            StopInvocation();
        }

        public void StopInvocation()
        {
            OnInvocationStopping();
            _lastInvocation = null;
            _clickLimit = DateTime.MinValue;
        }

        protected void Invoke(Vector2i cell)
        {
            OnNewInvocationData();

            _logger.LogMessage($"invoke on {cell}");

            var requestedArea = Range2i.FromMinAndMax(cell - _maxDistance, cell + _maxDistance + 1);
            using (var image = _editableImage.RequestAccess(requestedArea))
            {
                var givenArea = image.Area;
                if (!givenArea.Contains(cell))
                {
                    return;
                }

                PrepareWithClickedValue(image[cell]);
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
                        image[index] = Invoke(image[index], cellStrength);
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

        protected virtual MaskValues Invoke(MaskValues value, int localStrength) => value;
    }
}
