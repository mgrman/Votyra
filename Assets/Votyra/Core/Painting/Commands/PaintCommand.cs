using System;
using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.Models;

namespace Votyra.Core.Painting.Commands
{
    public abstract class PaintCommand : IPaintCommand
    {
        private readonly IEditableImage2f _editableImage;
        

        private IEditableMask2e _editableMask;

        private IThreadSafeLogger _logger;

        private (Vector2i cell, int maxStrength)? _lastInvocation;


        protected PaintCommand(IEditableImage2f editableImage, IEditableMask2e editableMask, IThreadSafeLogger logger)
        {
            _editableImage = editableImage;
            _editableMask = editableMask;
            _logger = logger;
            
        }

        public virtual void UpdateInvocationValues(Vector2i cell, int maxStrength)
        {
            if (_lastInvocation == null)
                Invoke(cell, maxStrength);


            var next = (cell, maxStrength);
            var previous = _lastInvocation ?? next;
            _lastInvocation = next;

            InvokeOnPath(previous, next);
        }

        public void StopInvocation()
        {
            OnInvocationStopping();
            _lastInvocation = null;
        }

        protected void InvokeOnPath((Vector2i cell, int maxStrength) from, (Vector2i cell, int maxStrength) to)
        {
            _logger.LogMessage($"from:{from} to:{to}");
            if (from.cell == to.cell)
            {
            }
            else if (from.cell.X == to.cell.X)
            {
                InvokeOnPathVertical(from, to);
            }
            else
            {
                InvokeOnPathNotVertical(from, to);
            }
        }

        protected void InvokeOnPathNotVertical((Vector2i cell, int maxStrength) from, (Vector2i cell, int maxStrength) to)
        {
            var deltax = to.cell.X - from.cell.X;
            var deltay = to.cell.Y - from.cell.Y;
            var deltaerr = Math.Abs(deltay / deltax); // if vertical this would be division by 0
            var error = 0.0f;
            var y = from.cell.Y;
            var signX = Math.Sign(to.cell.X - from.cell.X);
            for (var x = from.cell.X; x != to.cell.X; x += signX)
            {
                if (x != from.cell.X || y != from.cell.Y)
                    Invoke(new Vector2i(x, y), to.maxStrength);
                error = error + deltaerr;
                if (error >= 0.5f)
                {
                    y = y + Math.Sign(deltay) * 1;
                    error = error - 1.0f;
                }
            }

            Invoke(to.cell, to.maxStrength);
        }

        protected void InvokeOnPathVertical((Vector2i cell, int maxStrength) from, (Vector2i cell, int maxStrength) to)
        {
            var x = from.cell.X;
            var signY = Math.Sign(to.cell.Y - from.cell.Y);
            for (var y = from.cell.Y + signY; y != to.cell.Y; y += signY)
            {
                Invoke(new Vector2i(x, y), to.maxStrength);
            }

            Invoke(to.cell, to.maxStrength);
        }

        protected void Invoke(Vector2i cell, int maxStrength)
        {
            _logger.LogMessage($"invoke on {cell}");
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
                    givenArea.ForeachPointExlusive(index =>
                    {
                        var ox = index.X - cell.X;
                        var oy = index.Y - cell.Y;

                        var cellStrength = (absStrength - Math.Max(Math.Abs(ox), Math.Abs(oy))) * Math.Sign(maxStrength);
                        image[index] = Invoke(image[index], cellStrength);
                        mask[index] = Invoke(mask[index], cellStrength);
                    });
                }
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

        protected virtual void PrepareWithClickedValue(float clickedValue)
        {
        }

        protected virtual float Invoke(float value, int localStrength) => value;

        protected virtual MaskValues Invoke(MaskValues value, int localStrength) => value;
    }
}