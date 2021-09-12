using System;
using System.Linq;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Images
{
    public class InitialStateSetter2f
    {
        public InitialStateSetter2f(IEditableImage2f editableImage, IInitialImageConfig imageConfig, [Inject(Id = "root")] GameObject root)
        {
            FillInitialState(editableImage, imageConfig, root);
        }

        public void FillInitialState(IEditableImage2f editableImage, IInitialImageConfig imageConfig, GameObject root)
        {
            if (editableImage == null)
                return;
            if (imageConfig.InitialData is Texture2D)
                FillInitialState(editableImage,  imageConfig.InitialData as Texture2D, imageConfig.InitialDataScale.Z);
            if (imageConfig.InitialData is GameObject)
                FillInitialState(editableImage, (imageConfig.InitialData as GameObject).GetComponentsInChildren<Collider>(), imageConfig.InitialDataScale.Z, root);
            if (imageConfig.InitialData is Collider)
                FillInitialState(editableImage, new[] {imageConfig.InitialData as Collider}, imageConfig.InitialDataScale.Z, root);
            if (imageConfig.InitialData is IMatrix2<float>)
                FillInitialState(editableImage, imageConfig.InitialData as IMatrix2<float>, imageConfig.InitialDataScale.Z);
            if (imageConfig.InitialData is IMatrix3<bool>)
                FillInitialState(editableImage, imageConfig.InitialData as IMatrix3<bool>, imageConfig.InitialDataScale.Z);
        }

        private static void FillInitialState(IEditableImage2f editableImage, Texture2D texture, float scale)
        {
            using (var imageAccessor = editableImage.RequestAccess(Range2i.All))
            {
                Range2i matrixAreaToFill;
                if (imageAccessor.Area == Range2i.All)
                    matrixAreaToFill = new Vector2i(texture.width, texture.height).ToRange2i();
                else
                    matrixAreaToFill = imageAccessor.Area;

                var matrixSizeX = matrixAreaToFill.Size.X;
                var matrixSizeY = matrixAreaToFill.Size.Y;

                var min = matrixAreaToFill.Min;
                for (var ix = 0; ix < matrixAreaToFill.Size.X; ix++)
                {
                    for (var iy = 0; iy < matrixAreaToFill.Size.Y; iy++)
                    {
                        var pos = new Vector2i(ix, iy) + min;
                        var value = texture.GetPixelBilinear((float) pos.X / matrixSizeX, (float) pos.Y / matrixSizeY)
                            .grayscale * scale;
                        var height = value;
                        imageAccessor[pos] = height;
                    }
                }

            }
        }

        private static void FillInitialState(IEditableImage2f editableImage, Collider[] colliders, float scale, GameObject root)
        {
            var bounds = colliders.Select(o => o.bounds)
                .Select(o => Area3f.FromMinAndSize(o.min.ToVector3f(), o.size.ToVector3f()))
                .DefaultIfEmpty(Area3f.zero)
                .Aggregate((a, b) => a.Encapsulate(b));

            using (var imageAccessor = editableImage.RequestAccess(Range2i.All))
            {
                var area = imageAccessor.Area;
                var min = area.Min;
                for (var ix = 0; ix < area.Size.X; ix++)
                {
                    for (var iy = 0; iy < area.Size.Y; iy++)
                    {
                        var localPos=new Vector2i(ix, iy)+min;
                        var ray = new Ray(root.transform.TransformPoint(new Vector3(localPos.X, localPos.Y, bounds.Max.Z)), root.transform.TransformDirection(new Vector3(0, 0, -1)));

                        var value = colliders.Select(collider =>
                            {
                                RaycastHit hit;
                                if (collider.Raycast(ray, out hit, bounds.Size.Z))
                                    return Mathf.Max(0, bounds.Max.Z - hit.distance);
                                return 0;
                            })
                            .DefaultIfEmpty(0)
                            .Max() * scale;

                        imageAccessor[localPos] = value;
                    }
                }
            }
        }

        private static void FillInitialState(IEditableImage2f editableImage, IMatrix2<float> texture, float scale)
        {
            using (var imageAccessor = editableImage.RequestAccess(Range2i.All))
            {
                Range2i matrixAreaToFill;
                if (imageAccessor.Area == Range2i.All)
                    matrixAreaToFill = texture.Size.ToRange2i();
                else
                    matrixAreaToFill = imageAccessor.Area;
               
                var min = matrixAreaToFill.Min;
                for (var ix = 0; ix < matrixAreaToFill.Size.X; ix++)
                {
                    for (var iy = 0; iy < matrixAreaToFill.Size.Y; iy++)
                    {
                        var i=new Vector2i(ix, iy)+min;
                        imageAccessor[i] = texture[i] * scale;
                    }
                }
            }
        }

        private static void FillInitialState(IEditableImage2f editableImage, IMatrix3<bool> texture, float scale)
        {
            using (var imageAccessor = editableImage.RequestAccess(Range2i.All))
            {
                Range2i matrixAreaToFill;
                if (imageAccessor.Area == Range2i.All)
                    matrixAreaToFill = texture.Size.XY.ToRange2i();
                else
                    matrixAreaToFill = imageAccessor.Area;
                var min = matrixAreaToFill.Min;
                for (var ix = 0; ix < matrixAreaToFill.Size.X; ix++)
                {
                    for (var iy = 0; iy < matrixAreaToFill.Size.Y; iy++)
                    {
                        var i=new Vector2i(ix, iy)+min;
                        float value = 0;

                        for (var iz = texture.Size.Z - 1; iz >= 0; iz--)
                        {
                            var iTexture = new Vector3i(i.X, i.Y, iz);
                            if (texture[iTexture])
                            {
                                value = iz;
                                break;
                            }
                        }

                        imageAccessor[i] = value * scale;
                    }
                }
            }
        }
    }
}