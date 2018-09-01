using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Zenject;

namespace Votyra.Core
{
    public class ClickToPaint2i : ITickable
    {
        [Inject(Id = "root")]
        protected GameObject _root;

        [Inject]
        protected IImageSampler2i _sampler;

        private const int maxDistBig = 4;

        private const int maxDistSmall = 1;

        private const float Period = 0.1f;

        private const float smoothSpeedRelative = 0.2f;

        private Height? _centerValueToReuse;

        [Inject]
        private IEditableImage2i _editableImage;

        [InjectOptional]
        private IEditableMask2e _editableMask;

        private Vector2i? lastCell;
        private float lastTime;

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

        private void OnCellClick(Vector2i cell)
        {
            //var cell = new Vector2i(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
            if (cell == lastCell || Time.time < lastTime + Period)
            {
                return;
            }
            lastCell = cell;
            lastTime = Time.time;

            var editableImage = _editableImage;
            var editableMask = _editableMask;
            if (editableImage == null)
            {
                return;
            }

            if (Input.GetButton("Modifier1"))
            {
                int maxDist = 4;

                using (var image = editableImage.RequestAccess(Range2i.FromCenterAndExtents(cell, new Vector2i(maxDist + 2, maxDist + 2))))
                {
                    using (var mask = editableMask?.RequestAccess(Range2i.FromCenterAndExtents(cell, new Vector2i(maxDist + 2, maxDist + 2))))
                    {
                        Height centerValue;
                        if (_centerValueToReuse.HasValue)
                        {
                            centerValue = _centerValueToReuse.Value;
                        }
                        else
                        {
                            centerValue = image[cell];
                            _centerValueToReuse = centerValue;
                        }

                        bool fillHole = Input.GetMouseButton(0);

                        for (int ox = -maxDist; ox <= maxDist; ox++)
                        {
                            for (int oy = -maxDist; oy <= maxDist; oy++)
                            {
                                var index = cell + new Vector2i(ox, oy);
                                var value = image[index];
                                image[index] = Height.Lerp(centerValue, value, smoothSpeedRelative);

                                if (mask != null)
                                {
                                    var maskValue = mask[index];
                                    if (fillHole)
                                    {
                                        mask[index] = MaskValues.Terrain;
                                    }
                                    else
                                    {
                                        mask[index] = MaskValues.Hole;
                                    }
                                }
                            }
                        }

                    }
                }
            }
            else
            {
                int multiplier = 0;
                bool isHole = Input.GetKey(KeyCode.H);

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

                using (var image = editableImage.RequestAccess(Range2i.FromCenterAndExtents(cell, new Vector2i(maxDist + 2, maxDist + 2))))
                {
                    using (var mask = editableMask?.RequestAccess(Range2i.FromCenterAndExtents(cell, new Vector2i(maxDist + 2, maxDist + 2))))
                    {

                        if (isHole)
                        {
                            var index = cell;
                            mask[index] = MaskValues.Hole;
                        }
                        else
                        {

                            for (int ox = -maxDist; ox <= maxDist; ox++)
                            {
                                for (int oy = -maxDist; oy <= maxDist; oy++)
                                {
                                    var index = cell + new Vector2i(ox, oy);

                                    var dist = Mathf.Max(Mathf.Abs(ox), Mathf.Abs(oy));
                                    var value = image[index];
                                    image[index] = value + (multiplier * (maxDist - dist)).CreateHeightDifference();

                                }
                            }
                        }
                    }
                }
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
    }
}