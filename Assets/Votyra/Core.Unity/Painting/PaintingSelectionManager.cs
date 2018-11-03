using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Painting.Commands;
using Zenject;

namespace Votyra.Core.Painting
{
    public class PaintingSelectionManager : ITickable
    {
        protected const int maxDistBig = 3;

        protected const int maxDistSmall = 1;

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

        public PaintingSelectionManager([Inject(Id = "root")]GameObject root, List<IPaintCommand> commands, IImageSampler2i sampler, IPaintingModel paintingModel, Flatten flatten, MakeOrRemoveHole makeOrRemoveHole, IncreaseOrDecrease increaseOrDecrease)
        {
            _root = root;
            _commands = commands;
            _sampler = sampler;
            _paintingModel = paintingModel;
            _flatten = flatten;
            _makeOrRemoveHole = makeOrRemoveHole;
            _increaseOrDecrease = increaseOrDecrease;
            _paintingModel.PaintCommands.OnNext(_commands);
        }

        public void Tick()
        {
            var isActive = IsCommandActive();
            if (isActive)
            {
                var imagePosition = GetImagePosition();
                var strength = GetMultiplier() * GetDistance();

                _paintingModel.PaintInvocationData.OnNext(new PaintInvocationData(strength, imagePosition));
            }
            else
            {
                _paintingModel.PaintInvocationData.OnNext(null);
            }
        }

        private bool IsCommandActive()
        {
            return Input.GetMouseButton(0) || Input.GetMouseButton(1);
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
            if (Input.GetMouseButton(0))
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        private int GetDistance()
        {
            if (Input.GetKey(KeyCode.LeftControl))
                return maxDistBig;
            else
                return maxDistSmall;
        }

        private IPaintCommand GetCommandToExecute()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                return _flatten;
            }
            else if (Input.GetKey(KeyCode.H))
            {
                return _makeOrRemoveHole;
            }
            else
            {
                return _increaseOrDecrease;
            }
        }
    }
}