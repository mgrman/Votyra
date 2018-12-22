using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.EventSystems;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Painting.Commands;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Painting
{
    public class PaintingSelectionManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        protected const int maxDistBig = 3;

        protected const int maxDistSmall = 1;

        [Inject(Id = "root")]
        protected GameObject _root;

        [Inject]
        protected IImageSampler2i _sampler;

        [Inject]
        protected IPaintingModel _paintingModel;

        private PointerEventData _activePointerData;

        [Inject]
        public void Initialize()
        {
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            var invocationData = GetInvocationDataFromPointer(_activePointerData);
            _paintingModel.PaintInvocationData
                .OnNext(invocationData);
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            if (_activePointerData != null)
            {
                // already active pointer
                return;
            }
            _activePointerData = eventData;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_activePointerData != eventData)
            {
                // other pointer than active one is in "up" mode
                return;
            }
            _activePointerData = null;
        }

        private static Vector2i GetImagePosition(PointerEventData eventData)
        {
            var cameraPosition = eventData.pressEventCamera.transform.position;
            Vector3 worldPosition = eventData.pointerCurrentRaycast.worldPosition;

            var ray = new Ray(cameraPosition, worldPosition - cameraPosition);
            var gameObject = eventData.pointerCurrentRaycast.gameObject;
            var collider = gameObject.GetComponent<Collider>();

            RaycastHit hitInfo;
            collider.Raycast(ray, out hitInfo, eventData.pointerCurrentRaycast.distance * 1.1f);

            var imagePosition = hitInfo.textureCoord;
            return imagePosition.ToVector2f().RoundToVector2i();
        }

        private PaintInvocationData? GetInvocationDataFromPointer(PointerEventData eventData)
        {
            if (eventData == null)
            {
                return null;
            }
            var imagePosition = GetImagePosition(eventData);
            var strength = GetMultiplier() * GetDistance();

            return new PaintInvocationData(strength, imagePosition);
        }

        private int GetMultiplier()
        {
            if (Input.GetButton("InverseModifier"))
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        private int GetDistance()
        {
            if (Input.GetButton("ExtendedModifier"))
                return maxDistBig;
            else
                return maxDistSmall;
        }
    }
}