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
        private const int MaxDistBig = 3;

        private const int MaxDistSmall = 1;

        private PointerEventData _activePointerData;

        [Inject]
        protected IPaintingModel PaintingModel;

        [InjectOptional]
        protected ITerrainUVPostProcessor UVToImage;

        public void OnPointerDown(PointerEventData eventData)
        {
            if(GUIUtility.hotControl!=0)
                return;
            
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

        protected void Update()
        {
            if (Input.GetButtonDown("ExtendedModifier"))
            {
                PaintingModel.IsExtendedModifierActive = true;
            }

            if (Input.GetButtonUp("ExtendedModifier"))
            {
                PaintingModel.IsExtendedModifierActive = false;
            }

            if (Input.GetButtonDown("InverseModifier"))
            {
                PaintingModel.IsInvertModifierActive = true;
            }
            if (Input.GetButtonUp("InverseModifier"))
            {
                PaintingModel.IsInvertModifierActive = false;
            }

            
            var invocationData = GetInvocationDataFromPointer(_activePointerData);
            var command = PaintingModel.SelectedPaintCommand;

            if (command != null && invocationData != null)
            {
                command.UpdateInvocationValues(invocationData.Value.ImagePosition, invocationData.Value.Strength);
            }
            else
            {
                command?.StopInvocation();
            }
        }

        private Vector2i? GetImagePosition(PointerEventData eventData)
        {
            var cameraPosition = eventData.pressEventCamera.transform.position;
            var worldPosition = eventData.pointerCurrentRaycast.worldPosition;

            var ray = new Ray(cameraPosition, worldPosition - cameraPosition);
            var pointedGameObject = eventData.pointerCurrentRaycast.gameObject;
            if (pointedGameObject == null)
                return null;
            var pointerCollider = pointedGameObject.GetComponent<Collider>();
            if (pointerCollider == null)
                return null;

            if (!pointerCollider.Raycast(ray, out var hitInfo, eventData.pointerCurrentRaycast.distance * 1.1f))
                return null;

            var textureCoordinates = hitInfo.textureCoord.ToVector2f();
            return (UVToImage?.ReverseUV(textureCoordinates) ?? textureCoordinates).RoundToVector2i();
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

        private  int GetMultiplier()=> PaintingModel.IsInvertModifierActive ? -1 : 1;

        private  int GetDistance()=>PaintingModel.IsExtendedModifierActive ? MaxDistBig : MaxDistSmall;
        
    }
}