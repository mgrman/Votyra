using System;
using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.Painting.Commands
{
    public class PaintCommand : IPaintCommand
    {
        private static readonly TimeSpan ClickDelay = TimeSpan.FromSeconds(0.2);
        private readonly IEditableImage2f _editableImage;

        private readonly IEditableMask2e _editableMask;

        private DateTime _clickLimit;

        private IThreadSafeLogger _logger;
        private int _maxStrength;

        protected PaintCommand(IEditableImage2f editableImage, IEditableMask2e editableMask, IThreadSafeLogger logger)
        {
            _editableImage = editableImage;
            _editableMask = editableMask;
            _logger = logger;
        }

        private Vector2i? _lastInvocation { get; set; }

        public virtual void UpdateInvocationValues(Vector2i cell, int maxStrength)
        {
            if (DateTime.Now < _clickLimit && (_lastInvocation == null || (_lastInvocation.Value - cell).ManhattanMagnitude() < 3))
                return;

            if (_lastInvocation == null)
                _clickLimit = DateTime.Now + ClickDelay;

            _maxStrength = maxStrength;
            Path2iUtils.InvokeOnPath(_lastInvocation, cell, Invoke);

            _lastInvocation = cell;
        }

        public void StopInvocation()
        {
            OnInvocationStopping();
            _lastInvocation = null;
        }

        protected void Invoke(Vector2i cell)
        {
            OnNewInvocationData();
            Invoke(cell, _maxStrength);
        }

        protected void Invoke(Vector2i cell, int maxStrength)
        {
            // _logger.LogMessage($"invoke on {cell}");
            var absStrength = Math.Abs(maxStrength);
            var absExtents = absStrength - 1;

            var requestedArea = Range2i.FromMinAndMax(cell - (absStrength - 1), cell + (absStrength - 1) + 1);
            using (var image = _editableImage.RequestAccess(requestedArea))
            {
                using (var mask = _editableMask?.RequestAccess(requestedArea))
                {
                    var givenArea = image.Area.IntersectWith(mask.Area);
                    if (!givenArea.Contains(cell))
                        return;

                    PrepareWithClickedValue(image[cell]);
                    var min = givenArea.Min;
                    var max = givenArea.Max;
                    for (var ix = min.X; ix < max.X; ix++)
                    {
                        for (var iy = min.Y; iy <= max.Y; iy++)
                        {
                            var index = new Vector2i(ix, iy);
                            var ox = index.X - cell.X;
                            var oy = index.Y - cell.Y;

                            var cellStrength = (absStrength - Math.Max(Math.Abs(ox), Math.Abs(oy))) * Math.Sign(maxStrength);
                            image[index] = Invoke(image[index], cellStrength);
                            mask[index] = Invoke(mask[index], cellStrength);
                        }
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

        public void Dispose()
        {
            StopInvocation();
        }
    }
}