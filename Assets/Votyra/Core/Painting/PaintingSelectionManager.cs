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

            if (activeInput.IsInputActive(InputActions.ExtendedModifier))
                _paintingModel.IsExtendedModifierActive = true;

            if (!activeInput.IsInputActive(InputActions.ExtendedModifier) && _previousInputActions.IsInputActive(InputActions.ExtendedModifier))
                _paintingModel.IsExtendedModifierActive = false;

            if (activeInput.IsInputActive(InputActions.InverseModifier))
                _paintingModel.IsInvertModifierActive = true;

            if (!activeInput.IsInputActive(InputActions.InverseModifier) && _previousInputActions.IsInputActive(InputActions.InverseModifier))
                _paintingModel.IsInvertModifierActive = false;

            var invocationData = GetInvocationDataFromPointer(inputRay);

            if (activeInput.IsInputActive(InputActions.Action) && command != null && invocationData != null)
            {
                command.UpdateInvocationValues(invocationData.Value.ImagePosition, invocationData.Value.Strength);
                _previousInputActions = activeInput;
                return true;
            }

            command?.StopInvocation();
            _previousInputActions = activeInput;
            return false;
        }

        private Vector2i? GetImagePosition(Ray3f cameraRay) =>
            _raycaster.Raycast(cameraRay)
                ?.XY()
                .RoundToVector2i();

        private PaintInvocationData? GetInvocationDataFromPointer(Ray3f cameraRay)
        {
            var imagePosition = GetImagePosition(cameraRay);
            if (imagePosition == null)
                return null;
            var strength = GetMultiplier() * GetDistance();
            return new PaintInvocationData(strength, imagePosition.Value);
        }

        private int GetMultiplier() => _paintingModel.IsInvertModifierActive ? -1 : 1;

        private int GetDistance() => _paintingModel.IsExtendedModifierActive ? MaxDistBig : MaxDistSmall;
    }
}