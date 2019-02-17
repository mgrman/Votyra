using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
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

        [Inject(Id = "root")]
        protected GameObject _root;

        [Inject]
        protected List<IInputHandler> _inputHandlers;

        // private Dictionary<int, PointerEventData> _pointersDictionary;

        protected void Update()
        {
            var invocationData = GetInvocationDataFromPointer(_activePointerData);
            if (invocationData == null)
            {
                return;
            }
            for (int i = 0; i < _inputHandlers.Count; i++)
            {
                var used = _inputHandlers[i]
                    .Update(invocationData);
                if (used)
                {
                    _activePointerData.Use();
                }
            }

            // if (_pointersDictionary == null && EventSystem.current != null && EventSystem.current.currentInputModule != null)
            // {
            //     var field = typeof(PointerInputModule).GetField("m_PointerData", BindingFlags.Instance | BindingFlags.NonPublic);
            //     _pointersDictionary = field.GetValue(EventSystem.current.currentInputModule as PointerInputModule) as Dictionary<int, PointerEventData>;
            // }
            //
            // if (_pointersDictionary == null)
            // {
            //     return;
            // }
            //
            // foreach (var pair in _pointersDictionary)
            // {
            //     var inputData = GetInvocationDataFromPointer(pair.Value);
            //     if (inputData == null)
            //     {
            //         continue;
            //     }
            //     for (int i = 0; i < _inputHandlers.Count; i++)
            //     {
            //         var used = _inputHandlers[i]
            //             .Update(inputData);
            //         if (used)
            //         {
            //             pair.Value.Use();
            //         }
            //     }
            // }
        }

        [CanBeNull]
        private InputData GetInvocationDataFromPointer(PointerEventData eventData)
        {
            if (eventData == null || eventData.pressEventCamera == null || !eventData.pointerPressRaycast.isValid)
            {
                return null;
            }

            var ray = eventData.pressEventCamera.ScreenPointToRay(eventData.position);

            var cameraPosition = _root.transform.InverseTransformPoint(ray.origin)
                .ToVector3f();
            var cameraDirection = _root.transform.InverseTransformDirection(ray.direction)
                .ToVector3f();

            var cameraRay = new Ray3f(cameraPosition, cameraDirection);

            Debug.DrawRay(_root.transform.TransformPoint(cameraRay.Origin.ToVector3()), _root.transform.TransformDirection(cameraRay.Direction.ToVector3()) * 100, Color.red);

            InputActions activeInputs = default(InputActions);
            var inputs = EnumUtilities.GetNamesAndValues<InputActions>();
            for (int i = 0; i < inputs.Count; i++)
            {
                activeInputs |= Input.GetButton(inputs[i]
                    .name)
                    ? inputs[i]
                        .value
                    : default(InputActions);
            }

            return new InputData(cameraRay, activeInputs);
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            if (GUIUtility.hotControl != 0)
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
    }
}