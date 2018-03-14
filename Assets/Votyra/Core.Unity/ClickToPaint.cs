using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Zenject;

namespace Votyra.Core
{
    public class ClickToPaint : ITickable
    {
        [Inject]
        private IEditableImage2f _editableImage;

        [Inject]
        protected IImageSampler2i _sampler;

        [Inject(Id = "root")]
        protected GameObject _root;


        private const float Period = 0.1f;
        private const int maxDistBig = 4;
        private const int maxDistSmall = 1;
        private const float smoothSpeedRelative = 0.2f;
        private const float smoothCutoff = smoothSpeedRelative / 2;

        private float lastTime;
        private Vector2i? lastCell;

        private float? _centerValueToReuse;




        public void Tick()
        {
            if (Input.GetButtonUp("Modifier1"))
            {
                _centerValueToReuse = null;
            }
            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
            {
                lastCell = null;
            }
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                ProcessMouseClick();
            }
        }

        private void ProcessMouseClick()
        {
            // Debug.LogFormat("OnMouseDown on tile.");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Casts the ray and get the first game object hit
            Physics.Raycast(ray, out hit);

            var localPosition = _root.transform.worldToLocalMatrix.MultiplyPoint(hit.point);

            var imagePosition = _sampler.WorldToImage(new Vector2f(localPosition.x, localPosition.y));
            OnCellClick(imagePosition);
        }

        private void OnCellClick(Vector2i cell)
        {
            //var cell = new Vector2i(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
            if (cell == lastCell || Time.time < lastTime + Period)
            {
                return;
            }
            lastCell = cell;
            lastTime = Time.time;
            Debug.Log($"CellClick:{cell} at {Time.time}");

            var editableImage = _editableImage;
            if (editableImage == null)
            {
                return;
            }

            if (Input.GetButton("Modifier1"))
            {
                int maxDist = 4;

                using (var image = editableImage.RequestAccess(Rect2i.CenterAndExtents(cell, new Vector2i(maxDist, maxDist))))
                {
                    float centerValue;
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