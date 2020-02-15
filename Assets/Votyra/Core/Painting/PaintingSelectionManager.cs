using System.Linq;
using Votyra.Core.InputHandling;
using Votyra.Core.Models;
using Votyra.Core.Painting.Commands;
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
        private (InputActions action, IPaintCommand command)[] _potentionalPaintCommands;

        public PaintingSelectionManager(IPaintingModel paintingModel, IRaycaster raycaster)
        {
            _paintingModel = paintingModel;
            _raycaster = raycaster;
            _potentionalPaintCommands = EnumUtilities.GetValues<InputActions>()
                .Select(o => (o, paintingModel.PaintCommands.FirstOrDefault(cmd => cmd.GetType()
                    .Name == o.ToString())))
                .Where(o => o.Item2 != null)
                .ToArray();
        }

        public bool Update(Ray3f inputRay, InputActions activeInput)
        {
            bool any = false;
            for (int i = 0; i < _potentionalPaintCommands.Length; i++)
            {
                var potentionalPaintCommand = _potentionalPaintCommands[i];
                if (activeInput.IsInputActive(potentionalPaintCommand.action))
                {
                    _paintingModel.SelectedPaintCommand = potentionalPaintCommand.command;
                    any = true;
                }
            }

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
                    }

                    _previousInputActions = activeInput;
                    any = true;
                }
                else
                {
                    command.StopInvocation();
                }
            }

            _previousInputActions = activeInput;
            return any;
        }

        private PaintInvocationData? GetInvocationDataFromPointer(Ray3f cameraRay)
        {
            var rayHit = _raycaster.Raycast(cameraRay);
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