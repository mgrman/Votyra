using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Painting.Commands;
using Zenject;

namespace Votyra.Core.Painting
{
    public class PaintingSelectionManager : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler
    {
        protected const int maxDistBig = 3;

        protected const int maxDistSmall = 1;

        [Inject(Id = "root")]
        protected GameObject _root;

        [Inject]
        protected List<IPaintCommand> _commands;

        [Inject]
        protected IImageSampler2i _sampler;

        [Inject]
        protected IPaintingModel _paintingModel;

        [Inject]
        protected Flatten _flatten;

        [Inject]
        protected MakeOrRemoveHole _makeOrRemoveHole;

        [Inject]
        protected IncreaseOrDecrease _increaseOrDecrease;

        private bool _isActive;

        [Inject]
        public void Initialize()
        {
        }

        public void Update()
        {
            var isActive = IsCommandActive();
            if (isActive)
            {
            }
            else
            {
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            var imagePosition = GetImagePosition();
            var strength = GetMultiplier() * GetDistance();

            _paintingModel.PaintInvocationData.OnNext(new PaintInvocationData(strength, imagePosition));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _paintingModel.PaintInvocationData.OnNext(null);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _paintingModel.PaintInvocationData.OnNext(null);
        }

        private bool IsCommandActive()
        {
            return _isActive;
        }

        private Vector2i GetImagePosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Casts the ray and get the first game object hit
            Physics.Raycast(ray, out hit);

            var localPosition = _root.transform.worldToLocalMatrix.MultiplyPoint(hit.point);

            return _sampler.WorldToImage(new Vector2f(localPosition.x, localPosition.y));
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