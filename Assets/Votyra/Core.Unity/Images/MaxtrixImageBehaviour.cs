using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.Images
{
    public class MaxtrixImageBehaviour : MonoBehaviour, IImage2fProvider
    {
        public Texture2D InitialValueTexture = null;
        public float InitialValueScale = 1;

        private Matrix2<float> _editableMatrix;

        private Rect2i? _invalidatedArea;

        private readonly List<LockableMatrix2<float>> _readonlyMatrices = new List<LockableMatrix2<float>>();

        private MatrixImage2f _image = null;

        public IImage2f CreateImage()
        {
            if (_invalidatedArea.HasValue)
            {
                // Debug.LogFormat("Update readonlyCount:{0}", _readonlyMatrices.Count);

                var readonlyMatrix = _readonlyMatrices.FirstOrDefault(o => !o.IsLocked);
                if (readonlyMatrix == null)
                {
                    readonlyMatrix = new LockableMatrix2<float>(_editableMatrix.size);
                    _readonlyMatrices.Add(readonlyMatrix);
                }

                //sync
                for (int x = 0; x < _editableMatrix.size.x; x++)
                {
                    for (int y = 0; y < _editableMatrix.size.y; y++)
                    {
                        readonlyMatrix[x, y] = _editableMatrix[x, y];
                    }
                }

                // Debug.LogError($"_readonlyMatrices: {_readonlyMatrices.Count}");

                var oldImage = _image;
                oldImage?.Dispose();

                _image = new MatrixImage2f(readonlyMatrix, _invalidatedArea.Value);
                _invalidatedArea = null;
            }
            return _image;
        }

        private void Start()
        {
            if (InitialValueTexture == null)
            {
                var size = new Vector2i(10, 10);
                _editableMatrix = new Matrix2<float>(size);
                _invalidatedArea = new Rect2i(0, 0, 10, 10);
            }
            else
            {
                var texture = InitialValueTexture;

                int width = texture.width.FloorTo2();
                int height = texture.height.FloorTo2();

                var size = new Vector2i(width, height);
                _editableMatrix = new Matrix2<float>(size);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        _editableMatrix[x, y] = (int)(texture.GetPixel(x, y).grayscale * InitialValueScale);
                    }
                }
                FixImage(new Rect2i(0, 0, size.x, size.y), Direction.Unknown);
            }
        }

        private void Update()
        {
        }

        public MatrixImageAccessor RequestAccess(Rect2i area)
        {
            return new MatrixImageAccessor(this, area);
        }

        private enum Direction
        {
            Unknown = 0,
            Up = 1,
            Down = -1
        }

        private void FixImage(Rect2i area, Direction direction)
        {
            _invalidatedArea = _invalidatedArea?.CombineWith(area) ?? area;

            if (direction != Direction.Up && direction != Direction.Down)
            {
                direction = Direction.Down;
            }

            const int maxDiference = 1;

            Func<float, float, float> op;
            Func<float, float> getLimit;
            if (direction == Direction.Up)
            {
                op = Math.Max;
                getLimit = l => l - maxDiference;
            }
            else
            {
                op = Math.Min;
                getLimit = l => l + maxDiference;
            }

            for (int ix = area.xMin.FloorTo2(); ix < area.xMax.CeilTo2(); ix += 2)
            {
                for (int iy = area.yMin.FloorTo2(); iy < area.yMax.CeilTo2(); iy += 2)
                {
                    float x0y0 = _editableMatrix[ix + 0, iy + 0];
                    float x0y1 = _editableMatrix[ix + 0, iy + 1];
                    float x1y0 = _editableMatrix[ix + 1, iy + 0];
                    float x1y1 = _editableMatrix[ix + 1, iy + 1];

                    float limit = getLimit(op(op(op(x0y0, x0y1), x1y0), x1y1));

                    _editableMatrix[ix + 0, iy + 0] = op(_editableMatrix[ix + 0, iy + 0], limit);
                    _editableMatrix[ix + 0, iy + 1] = op(_editableMatrix[ix + 0, iy + 1], limit);
                    _editableMatrix[ix + 1, iy + 0] = op(_editableMatrix[ix + 1, iy + 0], limit);
                    _editableMatrix[ix + 1, iy + 1] = op(_editableMatrix[ix + 1, iy + 1], limit);
                }
            }
        }

        public class MatrixImageAccessor : IDisposable
        {
            private readonly MaxtrixImageBehaviour _behaviour;
            private readonly Matrix2<float> _editableMatrix;

            public Rect2i Area { get; }

            private float _changeCounter;

            public MatrixImageAccessor(MaxtrixImageBehaviour behaviour, Rect2i area)
            {
                _behaviour = behaviour;
                _editableMatrix = behaviour._editableMatrix;
                Area = area;
            }

            public float this[Vector2i pos]
            {
                get { return GetValue(pos); }
                set { SetValue(pos, value); }
            }

            public float GetValue(Vector2i pos)
            {
                if (!Area.Contains(pos))
                {
                    Debug.LogWarningFormat("Position {0} is outside of bounds!", pos);
                    return 0;
                }

                // Debug.LogFormat("Setting value at {0} to {1} readonlyCount:{2}", pos, value, _readonlyMatrices.Count);

                return _editableMatrix[pos];
            }

            public void SetByOffsetValue(Vector2i pos, float value)
            {
                if (!Area.Contains(pos))
                {
                    Debug.LogWarningFormat("Position {0} is outside of bounds!", pos);
                    return;
                }
                _changeCounter += value;
                _editableMatrix[pos] = _editableMatrix[pos] + value;
            }

            public void SetValue(Vector2i pos, float value)
            {
                if (!Area.Contains(pos))
                {
                    Debug.LogWarningFormat("Position {0} is outside of bounds!", pos);
                    return;
                }

                _changeCounter += _editableMatrix[pos] - value;
                _editableMatrix[pos] = value;
            }

            public void Dispose()
            {
                _behaviour.FixImage(Area, (Direction)Math.Sign(_changeCounter));
            }
        }
    }
}