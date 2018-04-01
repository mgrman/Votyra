using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Zenject;
using Votyra.Core.Utils;
using System.Collections.Generic;

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

        Stack<Renderer> _usedDebugObjects = new Stack<Renderer>();
        Stack<Renderer> _emptyDebugObjects = new Stack<Renderer>();
        Material _trueMaterial;
        Material _falseMaterial;

        private void DebugMouse()
        {
            var imageMousePosition = MouseImagePosition();


            var temp = _emptyDebugObjects;
            _emptyDebugObjects = _usedDebugObjects;
            _usedDebugObjects = temp;
            foreach (var toDelete in _usedDebugObjects)
            {
                GameObject.Destroy(toDelete.gameObject);
            }
            _usedDebugObjects.Clear();


            var area = Rect3i.CenterAndExtents(imageMousePosition, new Vector3i(2, 2, 2));
            using (var image = _editableImage.RequestAccess(area))
            {
                area.ForeachPoint((imagePosition) =>
                {

                    var value = image[imagePosition];
                    var worldPosition = ImageToWorld(imagePosition);
                    CreateDebugObjectAt(worldPosition, value);
                });
            }
        }

        private void CreateDebugObjectAt(Vector3f worldPos, bool value)
        {
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
            return ScreenToImage(Input.mousePosition);
        }
        private Vector3i ScreenToImage(Vector3 screenPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Casts the ray and get the first game object hit
            Physics.Raycast(ray, out hit);

            var localPosition = _root.transform.InverseTransformPoint(hit.point);

            var imagePosition = _sampler.WorldToImage(new Vector3f(localPosition.x, localPosition.y, localPosition.z));
            return imagePosition;
        }

        private Vector3f ImageToWorld(Vector3i imagePosition)
        {
            var localPosition = _sampler.ImageToWorld(imagePosition);
            var worldPosition = _root.transform.TransformPoint(localPosition.ToVector3());
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
                maxDist++;
                var area = Rect3i.CenterAndExtents(cell, new Vector3i(maxDist, maxDist, maxDist));
                using (var image = editableImage.RequestAccess(area))
                {
                    area = area.IntersectWith(image.Area);
                    if (value == true)
                    {
                        image[cell] = true;
                        image[cell + new Vector3i(0, 0, 1)] = true;
                    }
                    else if (value == false)
                    {
                        image[cell] = false;
                        image[cell + new Vector3i(0, 0, 1)] = false;
                    }
                }
            }
        }
    }
}