using System;
using System.Collections.Generic;
using System.Linq;
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
    public abstract class PaintCommand : IPaintCommand, ITickable
    {
        protected int _maxStrength;
        protected Vector2i _cell;

        protected virtual int PeriodMs { get; } = 200;

        [InjectOptional]
        protected IEditableMask2e _editableMask;

        [Inject]
        protected readonly IEditableImage2i _editableImage;

        private readonly object _invokeLock = new object();

        private LinkedList<(Vector2i cell, int maxStrength)> _invocationPath = new LinkedList<(Vector2i cell, int maxStrength)>();

        private (Vector2i cell, int maxStrength)? _lastInvocation;
        public virtual void Selected()
        {
        }

        public virtual void Unselected()
        {
        }

        public virtual void UpdateInvocationValues(Vector2i cell, int maxStrength)
        {
            _invocationPath.AddLast((cell, maxStrength));
            if (_lastInvocation == null)
            {
                Invoke(cell, maxStrength);
            }
        }

        public void Tick()
        {
            if (_invocationPath.Any())
            {
                var next = _invocationPath.First.Value;
                var previous = _lastInvocation ?? next;
                _invocationPath.RemoveFirst();
                _lastInvocation = next;

                InvokeOnPath(previous, next);
            }
        }

        protected void InvokeOnPath((Vector2i cell, int maxStrength) from, (Vector2i cell, int maxStrength) to)
        {
            Debug.Log($"from:{from} to:{to}");
            if (from.cell == to.cell)
            {
                return;
            }
            else if (from.cell.X == to.cell.X)
            {
                InvokeOnPathVertical(from, to);
                return;
            }
            else
            {
                InvokeOnPathNotVertical(from, to);
                return;
            }
        }

        protected void InvokeOnPathNotVertical((Vector2i cell, int maxStrength) from, (Vector2i cell, int maxStrength) to)
        {
            var deltax = to.cell.X - from.cell.X;
            var deltay = to.cell.Y - from.cell.Y;
            var deltaerr = Math.Abs(deltay / deltax);// if vertical this would be division by 0
            var error = 0.0f;
            var y = from.cell.Y;
            var signX = Math.Sign(to.cell.X - from.cell.X);
            for (int x = from.cell.X; x != to.cell.X; x += signX)
            {
                if (x != from.cell.X || y != from.cell.Y)
                {
                    Invoke(new Vector2i(x, y), to.maxStrength);
                }
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
            for (int y = from.cell.Y + signY; y != to.cell.Y; y += signY)
            {
                Invoke(new Vector2i(x, y), to.maxStrength);
            }
            Invoke(to.cell, to.maxStrength);
        }

        protected void Invoke(Vector2i cell, int maxStrength)
        {
            Debug.Log($"invoke on {cell}");
            var absStrength = Math.Abs(maxStrength);
            var absExtents = absStrength - 1;

            using (var image = _editableImage.RequestAccess(Range2i.FromCenterAndExtents(cell, new Vector2i(absStrength, absStrength))))
            {
                using (var mask = _editableMask?.RequestAccess(Range2i.FromCenterAndExtents(cell, new Vector2i(absStrength, absStrength))))
                {
                    PrepareWithClickedValue(image[cell]);
                    for (int ox = -absExtents; ox <= absExtents; ox++)
                    {
                        for (int oy = -absExtents; oy <= absExtents; oy++)
                        {
                            var index = cell + new Vector2i(ox, oy);
                            var cellStrength = (absStrength - Mathf.Max(Mathf.Abs(ox), Mathf.Abs(oy))) * Math.Sign(maxStrength);
                            image[index] = Invoke(image[index], cellStrength);
                            mask[index] = Invoke(mask[index], cellStrength);
                        }
                    }
                }
            }
        }

        public void StopInvocation()
        {
            OnInvocationStopping();
            _invocationPath.Clear();
            _lastInvocation = null;
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