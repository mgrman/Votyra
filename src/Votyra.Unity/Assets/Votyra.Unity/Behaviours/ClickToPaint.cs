using UnityEngine;
using System.Collections;
using Votyra.Common.Models;
using Votyra.Unity.Images;

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

        private MaxtrixImageBehaviour _image;

        private int? _centerValueToReuse;
        void Start()
        {
            _image = GetComponent<MaxtrixImageBehaviour>();
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


            if (Input.GetButton("Modifier1"))
            {
                int maxDist = 4;

                int centerValue;
                if (_centerValueToReuse.HasValue)
                {
                    centerValue = _centerValueToReuse.Value;
                }
                else
                {
                    centerValue = _image.GetValue(cell);
                    _centerValueToReuse = centerValue;
                }

                for (int ox = -maxDist; ox <= maxDist; ox++)
                {
                    for (int oy = -maxDist; oy <= maxDist; oy++)
                    {
                        var value = _image.GetValue(cell + new Vector2i(ox, oy));
                        var offsetF = (centerValue - value) * smoothSpeedRelative;
                        int offsetI = 0;
                        if (offsetF > smoothCutoff)
                            offsetI = Mathf.Max(1, Mathf.RoundToInt(offsetF));
                        else if (offsetF < -smoothCutoff)
                            offsetI = Mathf.Min(-1, Mathf.RoundToInt(offsetF));

                        _image.SetByOffsetValue(cell + new Vector2i(ox, oy), offsetI);
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

                for (int ox = -maxDist; ox <= maxDist; ox++)
                {
                    for (int oy = -maxDist; oy <= maxDist; oy++)
                    {
                        var dist = Mathf.Max(Mathf.Abs(ox), Mathf.Abs(oy));
                        _image.SetByOffsetValue(cell + new Vector2i(ox, oy), multiplier * (maxDist - dist));
                    }
                }
            }
        }
    }
}