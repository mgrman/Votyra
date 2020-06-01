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
        private readonly int maxDistance;

        private DateTime clickLimit;
        private IEditableImage2f editableImage;

        private IThreadSafeLogger logger;

        protected HolePaintCommand(int maxDistance)
        {
            this.maxDistance = maxDistance;
        }

        private Vector2i? LastInvocation { get; set; }

        public void Initialize(IEditableImage2f editableImage, IThreadSafeLogger logger)
        {
            this.editableImage = editableImage;
            this.logger = logger;
        }

        public virtual void UpdateInvocationValues(Vector2i cell)
        {
            var now = DateTime.UtcNow;
            if ((now < this.clickLimit) && ((this.LastInvocation == null) || ((this.LastInvocation.Value - cell).ManhattanMagnitude() < 3)))
            {
                return;
            }

            if (this.LastInvocation == null)
            {
                this.clickLimit = now + ClickDelay;
            }

            Path2iUtils.InvokeOnPath(this.LastInvocation, cell, this.Invoke);

            this.LastInvocation = cell;
        }

        public void Dispose()
        {
            this.StopInvocation();
        }

        public void StopInvocation()
        {
            this.OnInvocationStopping();
            this.LastInvocation = null;
            this.clickLimit = DateTime.MinValue;
        }

        protected void Invoke(Vector2i cell)
        {
            this.OnNewInvocationData();

            this.logger.LogInfo($"invoke on {cell}");

            var requestedArea = Range2i.FromMinAndMax(cell - this.maxDistance, cell + this.maxDistance + 1);
            using (var image = this.editableImage.RequestAccess(requestedArea))
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
