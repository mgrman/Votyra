using System.Linq;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Images
{
    public class InitialStateSetter2f
    {

        public InitialStateSetter2f(IEditableImage2f editableImage, IInitialImageConfig imageConfig, IImageSampler2i sampler, [Inject(Id = "root")]GameObject root)
        {
            FillInitialState(editableImage, imageConfig, sampler, root);
        }

        public void FillInitialState(IEditableImage2f editableImage, IInitialImageConfig imageConfig, IImageSampler2i sampler, GameObject root)
        {
            if (editableImage == null)
                return;
            if (imageConfig.InitialData is Texture2D)
            {
                FillInitialState(editableImage, imageConfig.InitialData as Texture2D, imageConfig.InitialDataScale.Z);
            }
            if (imageConfig.InitialData is GameObject)
            {
                FillInitialState(editableImage, (imageConfig.InitialData as GameObject).GetComponentsInChildren<Collider>(), imageConfig.InitialDataScale.Z, sampler, root);
            }
            if (imageConfig.InitialData is Collider)
            {
                FillInitialState(editableImage, new[] { imageConfig.InitialData as Collider }, imageConfig.InitialDataScale.Z, sampler, root);
            }
            if (imageConfig.InitialData is IMatrix2<float>)
            {
                FillInitialState(editableImage, imageConfig.InitialData as IMatrix2<float>, imageConfig.InitialDataScale.Z);
            }
            if (imageConfig.InitialData is IMatrix3<float>)
            {
                FillInitialState(editableImage, imageConfig.InitialData as IMatrix3<float>, imageConfig.InitialDataScale.Z);
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

                var matrixSizeX = matrixAreaToFill.Size.X;
                var matrixSizeY = matrixAreaToFill.Size.Y;

                for (int x = matrixAreaToFill.Min.X; x < matrixAreaToFill.Max.X; x++)
                {
                    for (int y = matrixAreaToFill.Min.Y; y < matrixAreaToFill.Max.Y; y++)
                    {
                        var pos = new Vector2i(x, y);
                        imageAccessor[pos] = texture.GetPixelBilinear((float)x / matrixSizeX, (float)y / matrixSizeY).grayscale * scale;
                    }
                }
            }
        }

        private static void FillInitialState(IEditableImage2f editableImage, Collider[] colliders, float scale, IImageSampler2i sampler, GameObject root)
        {
            var bounds = colliders.Select(o => o.bounds)
                .Select(o => Rect3f.FromMinAndSize(o.min.ToVector3f(), o.size.ToVector3f()))
                .DefaultIfEmpty(Rect3f.zero)
                .Aggregate((a, b) => a.Encapsulate(b));

            using (var imageAccessor = editableImage.RequestAccess(Rect2i.All))
            {
                var area = imageAccessor.Area;
                for (int ix = area.Min.X; ix < area.Max.X; ix++)
                {
                    for (int iy = area.Min.Y; iy < area.Max.Y; iy++)
                    {
                        var i = new Vector2i(ix, iy);
                        var localPos = sampler.ImageToWorld(i);

                        var ray = new Ray(root.transform.TransformPoint(new Vector3(localPos.X, localPos.Y, bounds.max.Z)), root.transform.TransformDirection(new Vector3(0, 0, -1)));

                        imageAccessor[i] = colliders
                            .Select(collider =>
                            {
                                RaycastHit hit;
                                if (collider.Raycast(ray, out hit, bounds.size.Z))
                                {
                                    return Mathf.Max(0, bounds.max.Z - hit.distance);
                                }
                                return 0;
                            })
                            .DefaultIfEmpty(0)
                            .Max() * scale;

                    }
                }
            }
        }

        private static void FillInitialState(IEditableImage2f editableImage, IMatrix2<float> texture, float scale)
        {
            using (var imageAccessor = editableImage.RequestAccess(Rect2i.All))
            {
                Rect2i matrixAreaToFill;
                if (imageAccessor.Area == Rect2i.All)
                {
                    matrixAreaToFill = texture.size.ToRect2i();
                }
                else
                {
                    matrixAreaToFill = imageAccessor.Area;
                }

                for (int x = matrixAreaToFill.Min.X; x < matrixAreaToFill.Max.X; x++)
                {
                    for (int y = matrixAreaToFill.Min.Y; y < matrixAreaToFill.Max.Y; y++)
                    {
                        var pos = new Vector2i(x, y);
                        imageAccessor[pos] = texture[x, y] * scale;
                    }
                }
            }
        }

        private static void FillInitialState(IEditableImage2f editableImage, IMatrix3<float> texture, float scale)
        {
            using (var imageAccessor = editableImage.RequestAccess(Rect2i.All))
            {
                Rect2i matrixAreaToFill;
                if (imageAccessor.Area == Rect2i.All)
                {
                    matrixAreaToFill = texture.size.XY.ToRect2i();
                }
                else
                {
                    matrixAreaToFill = imageAccessor.Area;
                }

                for (int x = matrixAreaToFill.Min.X; x < matrixAreaToFill.Max.X; x++)
                {
                    for (int y = matrixAreaToFill.Min.Y; y < matrixAreaToFill.Max.Y; y++)
                    {
                        var pos = new Vector2i(x, y);
                        float value = 0;

                        for (int z = texture.size.Z - 1; z >= 0; z--)
                        {
                            if (texture[x, y, z] > 0)
                            {
                                value = z;
                                break;
                            }
                        }
                        imageAccessor[pos] = value * scale;
                    }
                }
            }
        }
    }
}