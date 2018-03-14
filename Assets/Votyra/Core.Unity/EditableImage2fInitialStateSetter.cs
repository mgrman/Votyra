using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.Models;

namespace Votyra.Core.Images
{
    public class EditableImage2fInitialStateSetter
    {

        public EditableImage2fInitialStateSetter(IEditableImage2f editableImage, IInitialImageConfig imageConfig)
        {
            FillInitialState(editableImage, imageConfig);
        }

        public void FillInitialState(IEditableImage2f editableImage, IInitialImageConfig imageConfig)
        {
            if (editableImage == null)
                return;
            if (imageConfig.InitialData is Texture2D)
            {
                FillInitialState(editableImage, imageConfig.InitialData as Texture2D, imageConfig.InitialDataScale.z);
            }
        }

        private static void FillInitialState(IEditableImage2f editableImage, Texture2D texture, float scale)
        {
            using (var imageAccessor = editableImage.RequestAccess(Rect2i.All))
            {
                Rect2i matrixAreaToFill;
                if (imageAccessor.Area == Rect2i.All)
                {
                    matrixAreaToFill = new Vector2i(texture.width, texture.height).ToRect2i();
                }
                else
                {
                    matrixAreaToFill = imageAccessor.Area;
                }

                var matrixSizeX = matrixAreaToFill.size.x;
                var matrixSizeY = matrixAreaToFill.size.y;

                for (int x = matrixAreaToFill.xMin; x < matrixAreaToFill.xMax; x++)
                {
                    for (int y = matrixAreaToFill.yMin; y < matrixAreaToFill.yMax; y++)
                    {
                        var pos = new Vector2i(x, y);
                        imageAccessor[pos] = texture.GetPixelBilinear((float)x / matrixSizeX, (float)y / matrixSizeY).grayscale * scale;
                    }
                }
            }
        }

    }
}