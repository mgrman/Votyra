using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Votyra.Core.InputHandling;
using Votyra.Core.Models;
using Votyra.Core.Painting;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Unity.Painting
{
    public class UnityInputManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private PointerEventData _activePointerData;

        [Inject]
        protected List<IInputHandler> _inputHandlers;

        [Inject]
        protected ITerrainConfig _terrainConfig;

        private bool _processing;

        [Inject(Id = "root")]
        protected GameObject _root;

        private InputActions _bufferedInputs = default;
        private bool _invokeWithNull;

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (GUIUtility.hotControl != 0)
                return;

            if (_activePointerData != null)
                return;
            _activePointerData = eventData;
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (_activePointerData != eventData)
                return;
            _activePointerData = null;
        }

        protected void Update()
        {
            if (_processing)
            {
                _bufferedInputs = GetActiveInputs(_bufferedInputs);
                return;
            }

            _processing = true;

            var ray = GetRayFromPointer(_activePointerData);
            var activeInputs = GetActiveInputs(_bufferedInputs);
            if (!ray.Origin.AnyNan()) //action is when pointer is active
            {
                activeInputs |= InputActions.Action;
            }

            _bufferedInputs = default;

            if (activeInputs == default)
            {
                if (!_invokeWithNull)
                {
                    _processing = false;
                    return;
                }

                _invokeWithNull = false;
            }
            else
            {
                _invokeWithNull = true;
            }

            if (_terrainConfig.AsyncInput)
            {
                Task.Run(() =>
                {
                    InvokeHandlers(ray, activeInputs, _activePointerData);
                    _processing = false;
                });
            }
            else
            {
                InvokeHandlers(ray, activeInputs, _activePointerData);
                _processing = false;
            }
        }

        private void InvokeHandlers(Ray3f ray, InputActions activeInputs, PointerEventData pointerEventData)
        {
            for (var i = 0; i < _inputHandlers.Count; i++)
            {
                try
                {
                    var used = _inputHandlers[i]
                        .Update(ray, activeInputs);
                    if (used)
                        pointerEventData?.Use();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        private static InputActions GetActiveInputs(InputActions previousInputs)
        {
            var activeInputs = default(InputActions);
            var inputs = EnumUtilities.GetNamesAndValues<InputActions>();
            for (var i = 0; i < inputs.Count; i++)
            {
                var input = inputs[i];
                if (input.value == InputActions.Action) //action is handled via active pointer
                {
                    continue;
                }

                activeInputs |= Input.GetButton(input.name) ? input.value : default;
            }

            return activeInputs;
        }

        private Ray3f GetRayFromPointer(PointerEventData eventData)
        {
            if (eventData == null)
            {
                return new Ray3f(Vector3f.NaN, Vector3f.NaN);
            }

            var ray = eventData.pressEventCamera.ScreenPointToRay(eventData.position);

            var cameraPosition = _root.transform.InverseTransformPoint(ray.origin)
                .ToVector3f();
            var cameraDirection = _root.transform.InverseTransformDirection(ray.direction)
                .ToVector3f();

            var cameraRay = new Ray3f(cameraPosition, cameraDirection);

            Debug.DrawRay(_root.transform.TransformPoint(cameraRay.Origin.ToVector3()), _root.transform.TransformDirection(cameraRay.Direction.ToVector3()) * 100, Color.red);

            return cameraRay;
        }
    }
}