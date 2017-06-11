using UnityEngine;
using System.Collections;
using Votyra.Models;
using Votyra.Unity.Images;
using Votyra.Utils;

namespace Votyra.Unity.Behaviours
{
    public class ClickToPaint : MonoBehaviour
    {
        private const float Period = 0.1f;
        private const int maxDistBig = 4;
        private const int maxDistSmall = 1;
        private const float smoothSpeedRelative = 0.2f;
        private const float smoothCutoff = smoothSpeedRelative / 2;

        private float lastTime;
        private Vector2i lastCell;

        private TerrainGeneratorBehaviour _behaviour;

        private int? _centerValueToReuse;
        void Start()
        {
            _behaviour = GetComponent<TerrainGeneratorBehaviour>();
        }

        void Update()
        {
            if (Input.GetButtonUp("Modifier1"))
            {
                _centerValueToReuse = null;
            }
        }

        private void OnCellClick(Vector2 position)
        {
            var cell = new Vector2i(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
            if (cell == lastCell || Time.time < lastTime + Period)
            {
                return;
            }
            lastCell = cell;
            lastTime = Time.time;

            var editableImage = _behaviour.EditableImage;
            if (editableImage == null)
            {
                return;
            }

            if (Input.GetButton("Modifier1"))
            {
                int maxDist = 4;

                using (var image = editableImage.RequestAccess(Rect2i.CenterAndExtents(cell, new Vector2i(maxDist, maxDist))))
                {
                    int centerValue;
                    if (_centerValueToReuse.HasValue)
                    {
                        centerValue = _centerValueToReuse.Value;
                    }
                    else
                    {
                        centerValue = image[cell];
                        _centerValueToReuse = centerValue;
                    }

                    for (int ox = -maxDist; ox <= maxDist; ox++)
                    {
                        for (int oy = -maxDist; oy <= maxDist; oy++)
                        {
                            var index = cell + new Vector2i(ox, oy);
                            var value = image[index];
                            var offsetF = (centerValue - value) * smoothSpeedRelative;
                            int offsetI = 0;
                            if (offsetF > smoothCutoff)
                                offsetI = Mathf.Max(1, Mathf.RoundToInt(offsetF));
                            else if (offsetF < -smoothCutoff)
                                offsetI = Mathf.Min(-1, Mathf.RoundToInt(offsetF));

                            image[index] = value + offsetI;
                        }
                    }
                }
            }
            else
            {
                int multiplier = 0;

                if (Input.GetMouseButton(0))
                {
                    multiplier = 1;
                }
                else if (Input.GetMouseButton(1))
                {
                    multiplier = -1;
                }
                int maxDist;
                if (Input.GetButton("Modifier2"))
                    maxDist = maxDistBig;
                else
                    maxDist = maxDistSmall;

                using (var image = editableImage.RequestAccess(Rect2i.CenterAndExtents(cell, new Vector2i(maxDist, maxDist))))
                {
                    for (int ox = -maxDist; ox <= maxDist; ox++)
                    {
                        for (int oy = -maxDist; oy <= maxDist; oy++)
                        {
                            var index = cell + new Vector2i(ox, oy);

                            var dist = Mathf.Max(Mathf.Abs(ox), Mathf.Abs(oy));
                            var value = image[index];
                            image[index] = value + multiplier * (maxDist - dist);
                        }
                    }
                }
            }
        }
    }
}