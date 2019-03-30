using Votyra.Core.InputHandling;
using Votyra.Core.Models;
using Votyra.Core.Raycasting;
using Votyra.Core.Utils;

namespace Votyra.Core.Painting
{
    public class PaintingSelectionManager : IInputHandler
    {
        private const int MaxDistBig = 3;

        private const int MaxDistSmall = 1;

        private readonly IPaintingModel _paintingModel;

        private readonly IRaycaster _raycaster;

        private InputActions _previousInputActions;

        public PaintingSelectionManager(IPaintingModel paintingModel, IRaycaster raycaster)
        {
            _paintingModel = paintingModel;
            _raycaster = raycaster;
        }

        public bool Update(Ray3f inputRay, InputActions activeInput)
        {
            var command = _paintingModel.SelectedPaintCommand;
            if (command != null)
            {
                if (activeInput.IsInputActive(InputActions.ExtendedModifier))
                    _paintingModel.IsExtendedModifierActive = true;

                if (!activeInput.IsInputActive(InputActions.ExtendedModifier) && _previousInputActions.IsInputActive(InputActions.ExtendedModifier))
                    _paintingModel.IsExtendedModifierActive = false;

                if (activeInput.IsInputActive(InputActions.InverseModifier))
                    _paintingModel.IsInvertModifierActive = true;

                if (!activeInput.IsInputActive(InputActions.InverseModifier) && _previousInputActions.IsInputActive(InputActions.InverseModifier))
                    _paintingModel.IsInvertModifierActive = false;

                if (activeInput.IsInputActive(InputActions.Action))
                {
                    var invocationData = GetInvocationDataFromPointer(inputRay);
                    if (invocationData != null)
                    {
                        command.UpdateInvocationValues(invocationData.Value.ImagePosition, invocationData.Value.Strength);
                        _previousInputActions = activeInput;
                        return true;
                    }
                }

                command.StopInvocation();
            }

            _previousInputActions = activeInput;
            return false;
        }

        
        private PaintInvocationData? GetInvocationDataFromPointer(Ray3f cameraRay)
        {
            var rayHit= _raycaster.Raycast(cameraRay);
            if (rayHit.AnyNan())
            {
                return null;
            }
            var imagePosition = rayHit.XY()
                .RoundToVector2i();
            var strength = GetMultiplier() * GetDistance();
            return new PaintInvocationData(strength, imagePosition);
        }

        private int GetMultiplier() => _paintingModel.IsInvertModifierActive ? -1 : 1;

        private int GetDistance() => _paintingModel.IsExtendedModifierActive ? MaxDistBig : MaxDistSmall;
    }
}