using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Votyra.Core.InputHandling;
using Votyra.Core.Models;
using Votyra.Core.Painting;
using Votyra.Core.Painting.Commands;
using Votyra.Core.Raycasting;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Unity.Painting
{
    public class UnityInputManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private readonly List<ActionData> _activePointerDatas = new List<ActionData>();

        [Inject]
        protected IPaintingModel _paintingModel;

        [Inject]
        protected ITerrainConfig _terrainConfig;

        [Inject]
        protected IRaycaster _raycaster;

        private bool _processing;

        [Inject(Id = "root")]
        protected GameObject _root;

        private bool _invokeWithNull;
        private int _strength;

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (GUIUtility.hotControl != 0)
                return;

            var isFlat = Input.GetKey(KeyCode.LeftShift);
            var isHole = Input.GetKey(KeyCode.LeftControl);

            string cmdName;
            if (isFlat)
            {
                cmdName = KnownCommands.Flatten;
            }
            else if (isHole)
            {
                cmdName = KnownCommands.MakeOrRemoveHole;
            }
            else
            {
                cmdName = KnownCommands.IncreaseOrDecrease;
            }

            var factory = _paintingModel.PaintCommandFactories.FirstOrDefault(o => o.Action == cmdName);
            if (factory == null)
            {
                return;
            }

            var cmd = factory.Create();

            _activePointerDatas.Add(new ActionData(eventData, cmd));
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            for (var i = 0; i < _activePointerDatas.Count; i++)
            {
                var activePointerData = _activePointerDatas[i];

                if (activePointerData.EventData == eventData)
                {
                    activePointerData.Command.Dispose();
                    _activePointerDatas.RemoveAt(i);
                    return;
                }
            }
        }

        protected void Update()
        {
            if (_processing)
            {
                return;
            }

            if (_activePointerDatas.Count == 0)
            {
                return;
            }

            _processing = true;

            _strength = GetMultiplier() * GetDistance();

            for (var i = 0; i < _activePointerDatas.Count; i++)
            {
                var data = _activePointerDatas[i];
                data.Ray = GetRayFromPointer(data.EventData);
            }

            if (_terrainConfig.AsyncInput)
            {
                Task.Run(() =>
                {
                    try
                    {
                        ProcessInputs();
                        _processing = false;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                });
            }
            else
            {
                ProcessInputs();
                _processing = false;
            }
        }

        private void ProcessInputs()
        {
            for (var i = 0; i < _activePointerDatas.Count; i++)
            {
                var activePointerData = _activePointerDatas[i];

                var pointer = activePointerData.EventData;
                var cmd = activePointerData.Command;
                var ray = activePointerData.Ray;

                var cell = _raycaster.Raycast(ray)
                    .XY()
                    .RoundToVector2i();
                cmd.UpdateInvocationValues(cell, _strength);
            }
        }

        private int GetMultiplier() => Input.GetMouseButton(1) ? -1 : 1;

        private int GetDistance() => Input.GetKey(KeyCode.LeftAlt) ? 3 : 1;
        //
        // private void InvokeHandlers(Ray3f ray, InputActions activeInputs, PointerEventData pointerEventData)
        // {
        //     for (var i = 0; i < _inputHandlers.Count; i++)
        //     {
        //         try
        //         {
        //             var used = _inputHandlers[i]
        //                 .Update(ray, activeInputs);
        //             if (used)
        //                 pointerEventData?.Use();
        //         }
        //         catch (Exception ex)
        //         {
        //             Debug.LogException(ex);
        //         }
        //     }
        // }
        //
        // private static InputActions GetActiveInputs(InputActions previousInputs)
        // {
        //     var activeInputs = default(InputActions);
        //     var inputs = EnumUtilities.GetNamesAndValues<InputActions>();
        //     for (var i = 0; i < inputs.Count; i++)
        //     {
        //         var input = inputs[i];
        //         if (input.value == InputActions.Action) //action is handled via active pointer
        //         {
        //             continue;
        //         }
        //
        //         activeInputs |= Input.GetButton(input.name) ? input.value : default;
        //     }
        //
        //     return activeInputs;
        // }

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

    internal class ActionData
    {
        public ActionData(PointerEventData eventData, IPaintCommand command)
        {
            EventData = eventData;
            Command = command;
        }

        public PointerEventData EventData { get; }

        public IPaintCommand Command { get; }

        public Ray3f Ray { get; set; }
    }
}