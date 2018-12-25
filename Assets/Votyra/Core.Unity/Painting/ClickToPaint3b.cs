using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Logging;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core
{
    public class ClickToPaint3b : ITickable
    {
        [Inject]
        protected IImageSampler3 _sampler;

        [Inject(Id = "root")]
        protected GameObject _root;

        [Inject]
        protected IThreadSafeLogger _logger;

        private const float Period = 0.1f;

        private const int maxDistBig = 2;

        private const int maxDistSmall = 1;

        private const float smoothSpeedRelative = 0.2f;

        private const float smoothCutoff = smoothSpeedRelative / 2;

        [Inject]
        protected IEditableImage3b _editableImage;

        private float _lastTime;
        private Vector3i? _lastCell;

        private Stack<Renderer> _usedDebugObjects = new Stack<Renderer>();

        private Stack<Renderer> _emptyDebugObjects = new Stack<Renderer>();

        private Material _trueMaterial;

        private Material _falseMaterial;

        public void Tick()
        {
            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
            {
                _lastCell = null;
            }
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                ProcessMouseClick();
            }
            DebugMouse();
        }

        private void ProcessMouseClick()
        {
            OnCellClick(MouseImagePosition());
        }

        private void DebugMouse()
        {
            var mouseLocalPosition = MouseLocalPosition();
            var mouseImagePosition = LocalToImage(mouseLocalPosition);

            var temp = _emptyDebugObjects;
            _emptyDebugObjects = _usedDebugObjects;
            _usedDebugObjects = temp;
            foreach (var toDelete in _usedDebugObjects)
            {
                GameObject.Destroy(toDelete.gameObject);
            }
            _usedDebugObjects.Clear();

            var localArea = Area3f.FromCenterAndExtents(mouseLocalPosition, new Vector3f(1, 1, 1));
            var imageArea = _sampler.WorldToImage(localArea);
            using (var image = _editableImage.RequestAccess(imageArea))
            {
                localArea = localArea.IntersectWith(_sampler.ImageToWorld(image.Area));
                localArea.RoundToContain().ForeachPointExlusive((localPosition) =>
                {
                    var imagePosition_x0y0z0 = _sampler.CellToX0Y0Z0(localPosition);
                    var imagePosition_x0y0z1 = _sampler.CellToX0Y0Z1(localPosition);
                    var imagePosition_x0y1z0 = _sampler.CellToX0Y1Z0(localPosition);
                    var imagePosition_x0y1z1 = _sampler.CellToX0Y1Z1(localPosition);
                    var imagePosition_x1y0z0 = _sampler.CellToX1Y0Z0(localPosition);
                    var imagePosition_x1y0z1 = _sampler.CellToX1Y0Z1(localPosition);
                    var imagePosition_x1y1z0 = _sampler.CellToX1Y1Z0(localPosition);
                    var imagePosition_x1y1z1 = _sampler.CellToX1Y1Z1(localPosition);

                    var x0y0z0 = image[imagePosition_x0y0z0];
                    var x0y0z1 = image[imagePosition_x0y0z1];
                    var x0y1z0 = image[imagePosition_x0y1z0];
                    var x0y1z1 = image[imagePosition_x0y1z1];
                    var x1y0z0 = image[imagePosition_x1y0z0];
                    var x1y0z1 = image[imagePosition_x1y0z1];
                    var x1y1z0 = image[imagePosition_x1y1z0];
                    var x1y1z1 = image[imagePosition_x1y1z1];

                    CreateDebugObjectAt(ImageToWorld(imagePosition_x0y0z0) + LocalToWorldVector(new Vector3f(0.1f, 0.1f, 0)), x0y0z0);
                    // CreateDebugObjectAt(ImageToWorld(imagePosition_x0y0z1) + new Vector3f(0.1f, 0.1f, -0.1f), x0y0z1);
                    CreateDebugObjectAt(ImageToWorld(imagePosition_x0y1z0) + LocalToWorldVector(new Vector3f(0.1f, 0.4f, 0)), x0y1z0);
                    // CreateDebugObjectAt(ImageToWorld(imagePosition_x0y1z1) + new Vector3f(0.1f, -0.1f, -0.1f), x0y1z1);
                    CreateDebugObjectAt(ImageToWorld(imagePosition_x1y0z0) + LocalToWorldVector(new Vector3f(0.4f, 0.1f, 0)), x1y0z0);
                    // CreateDebugObjectAt(ImageToWorld(imagePosition_x1y0z1) + new Vector3f(-0.1f, 0.1f, -0.1f), x1y0z1);
                    CreateDebugObjectAt(ImageToWorld(imagePosition_x1y1z0) + LocalToWorldVector(new Vector3f(0.4f, 0.4f, 0)), x1y1z0);
                    // CreateDebugObjectAt(ImageToWorld(imagePosition_x1y1z1) + new Vector3f(-0.1f, -0.1f, -0.1f), x1y1z1);
                });
            }
        }

        private void CreateDebugObjectAt(Vector3f worldPos, bool value)
        {
            if (!value)
                return;

            var debugRenderer = _emptyDebugObjects.Count > 0 ? _emptyDebugObjects.Pop() : null;

            if (debugRenderer == null)
            {
                var debugObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                debugObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                debugObject.GetComponent<Collider>().enabled = false;
                debugObject.name = "Debug pointer";
                debugRenderer = debugObject.GetComponent<Renderer>();
                _trueMaterial = Resources.Load<Material>("PointerTrue");
                _falseMaterial = Resources.Load<Material>("PointerFalse");
            }
            debugRenderer.transform.position = worldPos.ToVector3();
            debugRenderer.material = value ? _trueMaterial : _falseMaterial;

            _usedDebugObjects.Push(debugRenderer);
        }

        private Vector3i MouseImagePosition()
        {
            return LocalToImage(ScreenToLocal(Input.mousePosition));
        }

        private Vector3f MouseLocalPosition()
        {
            return ScreenToLocal(Input.mousePosition);
        }

        private Vector3f ScreenToLocal(Vector3 screenPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Casts the ray and get the first game object hit
            Physics.Raycast(ray, out hit);

            var localPosition = _root.transform.InverseTransformPoint(hit.point);
            return new Vector3f(localPosition.x, localPosition.y, localPosition.z);
        }

        private Vector3i LocalToImage(Vector3f localPosition)
        {
            var imagePosition = _sampler.WorldToImage(localPosition);
            return imagePosition;
        }

        private Vector3f ImageToWorld(Vector3i imagePosition)
        {
            var localPosition = _sampler.ImageToWorld(imagePosition);
            var worldPosition = _root.transform.TransformPoint(localPosition.ToVector3());
            return worldPosition.ToVector3f();
        }

        private Vector3f LocalToWorldVector(Vector3f localPosition)
        {
            var worldPosition = _root.transform.TransformVector(localPosition.ToVector3());
            return worldPosition.ToVector3f();
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
                // int maxDist = 4;

                // var area = Rect3i.CenterAndExtents(cell, new Vector3i(maxDist, maxDist, maxDist));
                // using (var image = editableImage.RequestAccess(area))
                // {
                //     area = area.IntersectWith(image.Area);
                //     bool centerValue;
                //     if (_centerValueToReuse.HasValue)
                //     {
                //         centerValue = _centerValueToReuse.Value;
                //     }
                //     else
                //     {
                //         centerValue = image[cell];
                //         _centerValueToReuse = centerValue;
                //     }

                //     area.ForeachPoint(index =>
                //     {
                //         image[index] = centerValue;
                //     });
                // }
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

                var areaToChange = Range3i.FromCenterAndExtents(cell, Vector3i.FromSame(maxDist));

                var extendedSetArea = Range3i.FromMinAndSize(areaToChange.Min, areaToChange.Size + new Vector3i(0, 0, 1));
                using (var image = editableImage.RequestAccess(extendedSetArea))
                {
                    _logger.LogMessage($"image.Area:{image.Area}");
                    _logger.LogMessage($"areaToChange:{areaToChange}");
                    var actualAreaToChange = areaToChange.IntersectWith(image.Area);
                    _logger.LogMessage($"actualAreaToChange:{actualAreaToChange}");
                    actualAreaToChange.ForeachPointExlusive(point =>
                    {
                        _logger.LogMessage($"image[point] {image[point]} to {value}");
                        _logger.LogMessage($"image[point+ new Vector3i(0, 0, 1)] {image[point + new Vector3i(0, 0, 1)]} to {value}");
                        image[point] = value;
                        image[point + new Vector3i(0, 0, 1)] = value;
                    });
                }
            }
        }
    }
}