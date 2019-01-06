using UnityEngine;
using UnityEngine.EventSystems;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Painting
{
    public class PaintingSelectionManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        protected const int maxDistBig = 3;

        protected const int maxDistSmall = 1;

        private PointerEventData _activePointerData;

        [Inject]
        protected IPaintingModel _paintingModel;

        [InjectOptional]
        protected ITerrainUVPostProcessor _uvToImage;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_activePointerData != null)
                return;
            _activePointerData = eventData;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_activePointerData != eventData)
                return;
            _activePointerData = null;
        }

        [Inject]
        public void Initialize()
        {
        }

        /// <summary>
        ///     Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            var invocationData = GetInvocationDataFromPointer(_activePointerData);
            _paintingModel.PaintInvocationData.OnNext(invocationData);
        }

        private Vector2i? GetImagePosition(PointerEventData eventData)
        {
            var cameraPosition = eventData.pressEventCamera.transform.position;
            var worldPosition = eventData.pointerCurrentRaycast.worldPosition;

            var ray = new Ray(cameraPosition, worldPosition - cameraPosition);
            var gameObject = eventData.pointerCurrentRaycast.gameObject;
            if (gameObject == null)
                return null;
            var collider = gameObject.GetComponent<Collider>();
            if (collider == null)
                return null;

            RaycastHit hitInfo;
            if (!collider.Raycast(ray, out hitInfo, eventData.pointerCurrentRaycast.distance * 1.1f))
                return null;

            var textureCoord = hitInfo.textureCoord.ToVector2f();
            return (_uvToImage?.ReverseUV(textureCoord) ?? textureCoord).RoundToVector2i();
        }

        private PaintInvocationData? GetInvocationDataFromPointer(PointerEventData eventData)
        {
            if (eventData == null)
                return null;
            var imagePosition = GetImagePosition(eventData);
            if (imagePosition == null)
                return null;
            var strength = GetMultiplier() * GetDistance();
            return new PaintInvocationData(strength, imagePosition.Value);
        }

        private int GetMultiplier()
        {
            if (Input.GetButton("InverseModifier"))
                return -1;
            return 1;
        }

        private int GetDistance()
        {
            if (Input.GetButton("ExtendedModifier"))
                return maxDistBig;
            return maxDistSmall;
        }
    }
}