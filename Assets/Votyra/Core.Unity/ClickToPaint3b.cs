using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Zenject;

namespace Votyra.Core
{
    public class ClickToPaint3b : ITickable
    {
        [Inject]
        private IEditableImage3b _editableImage;

        [Inject]
        protected IImageSampler3 _sampler;

        [Inject(Id = "root")]
        protected GameObject _root;

        private const float Period = 0.1f;
        private const int maxDistBig = 4;
        private const int maxDistSmall = 1;
        private const float smoothSpeedRelative = 0.2f;
        private const float smoothCutoff = smoothSpeedRelative / 2;

        private float _lastTime;
        private Vector3i? _lastCell;

        private bool? _centerValueToReuse;

        public void Tick()
        {
            if (Input.GetButtonUp("Modifier1"))
            {
                _centerValueToReuse = null;
            }
            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
            {
                _lastCell = null;
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

            var imagePosition = _sampler.WorldToImage(new Vector3f(localPosition.x, localPosition.y, localPosition.z));

            OnCellClick(imagePosition);
        }

        private void OnCellClick(Vector3i cell)
        {
            //var cell = new Vector2i(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
            if (cell == _lastCell || Time.time < _lastTime + Period)
            {
                return;
            }
            _lastCell = cell;
            _lastTime = Time.time;

            var editableImage = _editableImage;
            if (editableImage == null)
            {
                return;
            }

            if (Input.GetButton("Modifier1"))
            {
                int maxDist = 4;

                var area = Rect3i.CenterAndExtents(cell, new Vector3i(maxDist, maxDist, maxDist));
                using (var image = editableImage.RequestAccess(area))
                {
                    area = area.IntersectWith(image.Area);
                    bool centerValue;
                    if (_centerValueToReuse.HasValue)
                    {
                        centerValue = _centerValueToReuse.Value;
                    }
                    else
                    {
                        centerValue = image[cell];
                        _centerValueToReuse = centerValue;
                    }

                    area.ForeachPoint(index =>
                    {
                        image[index] = centerValue;
                    });
                }
            }
            else
            {
                bool value = false;

                if (Input.GetMouseButton(0))
                {
                    value = true;
                }
                int maxDist;
                if (Input.GetButton("Modifier2"))
                    maxDist = maxDistBig;
                else
                    maxDist = maxDistSmall;

                var area = Rect3i.CenterAndExtents(cell, new Vector3i(maxDist, maxDist, maxDist));
                using (var image = editableImage.RequestAccess(area))
                {
                    area = area.IntersectWith(image.Area);
                    area.ForeachPoint(index =>
                    {
                        var dist = (index - cell).magnitudeManhatanRing;
                        image[index] = value;
                    });
                }
            }
        }
    }
}