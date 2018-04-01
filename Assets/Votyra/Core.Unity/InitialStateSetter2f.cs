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
                FillInitialState(editableImage, imageConfig.InitialData as Texture2D, imageConfig.InitialDataScale.z);
            }
            if (imageConfig.InitialData is GameObject)
            {
                FillInitialState(editableImage, (imageConfig.InitialData as GameObject).GetComponentsInChildren<Collider>(), imageConfig.InitialDataScale.z, sampler, root);
            }
            if (imageConfig.InitialData is Collider)
            {
                FillInitialState(editableImage, new[] { imageConfig.InitialData as Collider }, imageConfig.InitialDataScale.z, sampler, root);
            }
            if (imageConfig.InitialData is IMatrix2<float>)
            {
                FillInitialState(editableImage, imageConfig.InitialData as IMatrix2<float>, imageConfig.InitialDataScale.z);
            }
            if (imageConfig.InitialData is IMatrix3<float>)
            {
                FillInitialState(editableImage, imageConfig.InitialData as IMatrix3<float>, imageConfig.InitialDataScale.z);
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

        private static void FillInitialState(IEditableImage2f editableImage, Collider[] colliders, float scale, IImageSampler2i sampler, GameObject root)
        {
            var bounds = colliders.Select(o => o.bounds)
                .Select(o => new Rect3f(o.min.ToVector3f(), o.size.ToVector3f()))
                .DefaultIfEmpty(Rect3f.zero)
                .Aggregate((a, b) => a.Encapsulate(b));

            using (var imageAccessor = editableImage.RequestAccess(Rect2i.All))
            {
                var area = imageAccessor.Area;
                for (int ix = area.xMin; ix < area.xMax; ix++)
                {
                    for (int iy = area.yMin; iy < area.yMax; iy++)
                    {
                        var i = new Vector2i(ix, iy);
                        var localPos = sampler.ImageToWorld(i);

                        var ray = new Ray(root.transform.TransformPoint(new Vector3(localPos.x, localPos.y, bounds.max.z)), root.transform.TransformDirection(new Vector3(0, 0, -1)));

                        imageAccessor[i] = colliders
                            .Select(collider =>
                            {
                                RaycastHit hit;
                                if (collider.Raycast(ray, out hit, bounds.size.z))
                                {
                                    return Mathf.Max(0, bounds.max.z - hit.distance);
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

                for (int x = matrixAreaToFill.xMin; x < matrixAreaToFill.xMax; x++)
                {
                    for (int y = matrixAreaToFill.yMin; y < matrixAreaToFill.yMax; y++)
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

                for (int x = matrixAreaToFill.xMin; x < matrixAreaToFill.xMax; x++)
                {
                    for (int y = matrixAreaToFill.yMin; y < matrixAreaToFill.yMax; y++)
                    {
                        var pos = new Vector2i(x, y);
                        float value = 0;

                        for (int z = texture.size.z - 1; z >= 0; z--)
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