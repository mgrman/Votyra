using System;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.InputHandling;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.Painting
{
    public class PaintingSelectionManager:IInputHandler
    {
        private const int MaxDistBig = 3;

        private const int MaxDistSmall = 1;

        private readonly IPaintingModel _paintingModel;

        private readonly IImage2fProvider _image2FProvider;

        private InputActions _previousInputActions;

        public PaintingSelectionManager(IPaintingModel paintingModel, IImage2fProvider image2FProvider)
        {
            _paintingModel = paintingModel;
            _image2FProvider = image2FProvider;
        }

        public bool Update(InputData frameData)
        {
            var activeInput = frameData.InputActions;
            var command = _paintingModel.SelectedPaintCommand;
            
            if (!activeInput.IsInputActive(InputActions.Action))
            {
                command?.StopInvocation();
                _previousInputActions = activeInput;
                return false;
            }
            
            if (activeInput.IsInputActive(InputActions.ExtendedModifier))
            {
                _paintingModel.IsExtendedModifierActive = true;
            }
            
            if (!activeInput.IsInputActive(InputActions.ExtendedModifier) && _previousInputActions.IsInputActive(InputActions.ExtendedModifier))
            {
                _paintingModel.IsExtendedModifierActive = false;
            }

            if (activeInput.IsInputActive(InputActions.InverseModifier))
            {
                _paintingModel.IsInvertModifierActive = true;
            }
            
            if (!activeInput.IsInputActive(InputActions.InverseModifier) && _previousInputActions.IsInputActive(InputActions.InverseModifier))
            {
                _paintingModel.IsInvertModifierActive = false;
            }

            var invocationData = GetInvocationDataFromPointer(frameData.InputRay);

            if (command != null && invocationData != null)
            {
                command.UpdateInvocationValues(invocationData.Value.ImagePosition, invocationData.Value.Strength);
                _previousInputActions = activeInput;
                return true;
            }
            else
            {
                command?.StopInvocation();
                _previousInputActions = activeInput;
                return false;
            }

        }

        private Vector2i? GetImagePosition(Ray3f cameraRay)
        {
             return Raycast(cameraRay)
                 ?.RoundToVector2i();
        }

        private Vector2f? Raycast(Ray3f cameraRay)
        {
            float maxDistance = 500;
            var image = _image2FProvider.CreateImage();
            (image as IInitializableImage)?.StartUsing();

            var cameraRayXY = cameraRay.XY;
            
            var startXY = cameraRayXY.Origin;
            var directionNonNormalizedXY = cameraRay.Direction.XY;
            var directionXYMag = directionNonNormalizedXY.Magnitude;
            var endXY = (startXY + directionNonNormalizedXY.Normalized * maxDistance);

            float GetRayValue(Vector2f point)
            {
                var p = (point - startXY).Magnitude / directionXYMag;
                return cameraRay.Origin.Z + cameraRay.Direction.Z * p;
            }

            Vector2f? result = null;

            Path2fUtils.InvokeOnPath(startXY,
                endXY,
                (line) =>
                {
                    var fromImageValue = GetLinearInterpolatedValue(image, line.From);
                    var toImageValue = GetLinearInterpolatedValue(image, line.To);

                    var fromRayValue = GetRayValue(line.From);
                    var toRayValue = GetRayValue(line.To);

                    var x = (fromRayValue - fromImageValue) / (toImageValue - fromImageValue - toRayValue + fromRayValue);
                    if (x < 0 || x > 1)
                    {
                        return false;
                    }

                    result = line.From + (line.To - line.From) * x;
                    return true;
                });

            (image as IInitializableImage)?.FinishUsing();

            return result;
        }

        private float GetLinearInterpolatedValue(IImage2f image, Vector2f pos)
        {
            var pos_x0y0 = pos.FloorToVector2i();
            var fraction = pos - pos_x0y0;

            var pos_x0y1 = pos_x0y0 + new Vector2i(0, 1);
            var pos_x1y0 = pos_x0y0 + new Vector2i(1, 0);
            var pos_x1y1 = pos_x0y0 + new Vector2i(1, 1);

            var x0y0 = image.Sample(pos_x0y0);
            var x0y1 = image.Sample(pos_x0y1);
            var x1y0 = image.Sample(pos_x1y0);
            var x1y1 = image.Sample(pos_x1y1);

            return (1f - fraction.X) * (1f - fraction.Y) * x0y0 + fraction.X * (1f - fraction.Y) * x1y0 + (1f - fraction.X) * fraction.Y * x0y1 + fraction.X * fraction.Y * x1y1;
        }

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