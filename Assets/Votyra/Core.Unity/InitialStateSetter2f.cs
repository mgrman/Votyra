using System.Linq;
using UnityEngine;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Images
{
    public class InitialStateSetter2f
    {
        public InitialStateSetter2f(IEditableImage2i editableImage, [InjectOptional] IEditableMask2e editableMask, IInitialImageConfig imageConfig, [Inject(Id = "root")]GameObject root)
        {
            FillInitialState(editableImage, editableMask, imageConfig, root);
        }

        public void FillInitialState(IEditableImage2i editableImage, IEditableMask2e editableMask, IInitialImageConfig imageConfig, GameObject root)
        {
            if (editableImage == null)
                return;
            if (imageConfig.InitialData is Texture2D)
            {
                FillInitialState(editableImage, editableMask, imageConfig.InitialData as Texture2D, imageConfig.InitialDataScale.Z, imageConfig.ZeroFromInitialStateIsNull);
            }
            if (imageConfig.InitialData is GameObject)
            {
                FillInitialState(editableImage, (imageConfig.InitialData as GameObject).GetComponentsInChildren<Collider>(), imageConfig.InitialDataScale.Z, root);
            }
            if (imageConfig.InitialData is Collider)
            {
                FillInitialState(editableImage, new[] { imageConfig.InitialData as Collider }, imageConfig.InitialDataScale.Z, root);
            }
            if (imageConfig.InitialData is IMatrix2<float>)
            {
                FillInitialState(editableImage, imageConfig.InitialData as IMatrix2<float>, imageConfig.InitialDataScale.Z);
            }
            if (imageConfig.InitialData is IMatrix3<bool>)
            {
                FillInitialState(editableImage, imageConfig.InitialData as IMatrix3<bool>, imageConfig.InitialDataScale.Z);
            }
        }

        private static void FillInitialState(IEditableImage2i editableImage, IEditableMask2e editableMask, Texture2D texture, float scale, bool zeroIsNull)
        {
            using (var imageAccessor = editableImage.RequestAccess(Range2i.All))
            {
                using (var maskAccessor = editableMask?.RequestAccess(Range2i.All))
                {
                    Range2i matrixAreaToFill;
                    if (imageAccessor.Area == Range2i.All)
                    {
                        matrixAreaToFill = new Vector2i(texture.width, texture.height).ToRange2i();
                    }
                    else
                    {
                        matrixAreaToFill = imageAccessor.Area;
                    }

                    var matrixSizeX = matrixAreaToFill.Size.X;
                    var matrixSizeY = matrixAreaToFill.Size.Y;

                    matrixAreaToFill.ForeachPointExlusive(pos =>
                    {
                        var value = texture.GetPixelBilinear((float)pos.X / matrixSizeX, (float)pos.Y / matrixSizeY).grayscale * scale;
                        var height = value;
                        imageAccessor[pos] = height;
                        if (maskAccessor != null)
                        {
                            maskAccessor[pos] = (zeroIsNull && height == 0f) ? MaskValues.Hole : MaskValues.Terrain;
                        }
                    });
                }
            }
        }

        private static void FillInitialState(IEditableImage2i editableImage, Collider[] colliders, float scale, GameObject root)
        {
            var bounds = colliders.Select(o => o.bounds)
                .Select(o => Area3f.FromMinAndSize(o.min.ToVector3f(), o.size.ToVector3f()))
                .DefaultIfEmpty(Area3f.zero)
                .Aggregate((a, b) => a.Encapsulate(b));

            using (var imageAccessor = editableImage.RequestAccess(Range2i.All))
            {
                var area = imageAccessor.Area;
                area.ForeachPointExlusive(localPos =>
                {
                    var ray = new Ray(root.transform.TransformPoint(new Vector3(localPos.X, localPos.Y, bounds.Max.Z)), root.transform.TransformDirection(new Vector3(0, 0, -1)));

                    var value = colliders
                        .Select(collider =>
                        {
                            RaycastHit hit;
                            if (collider.Raycast(ray, out hit, bounds.Size.Z))
                            {
                                return Mathf.Max(0, bounds.Max.Z - hit.distance);
                            }
                            return 0;
                        })
                        .DefaultIfEmpty(0)
                        .Max() * scale;

                    imageAccessor[localPos] = value;
                });
            }
        }

        private static void FillInitialState(IEditableImage2i editableImage, IMatrix2<float> texture, float scale)
        {
            using (var imageAccessor = editableImage.RequestAccess(Range2i.All))
            {
                Range2i matrixAreaToFill;
                if (imageAccessor.Area == Range2i.All)
                {
                    matrixAreaToFill = texture.Size.ToRange2i();
                }
                else
                {
                    matrixAreaToFill = imageAccessor.Area;
                }
                matrixAreaToFill.ForeachPointExlusive(i =>
                {
                    imageAccessor[i] = texture[i] * scale;
                });
            }
        }

        private static void FillInitialState(IEditableImage2i editableImage, IMatrix3<bool> texture, float scale)
        {
            using (var imageAccessor = editableImage.RequestAccess(Range2i.All))
            {
                Range2i matrixAreaToFill;
                if (imageAccessor.Area == Range2i.All)
                {
                    matrixAreaToFill = texture.Size.XY.ToRange2i();
                }
                else
                {
                    matrixAreaToFill = imageAccessor.Area;
                }
                matrixAreaToFill.ForeachPointExlusive(i =>
                {
                    float value = 0;

                    for (int iz = texture.Size.Z - 1; iz >= 0; iz--)
                    {
                        var iTexture = new Vector3i(i.X, i.Y, iz);
                        if (texture[iTexture])
                        {
                            value = iz;
                            break;
                        }
                    }
                    imageAccessor[i] = value * scale;
                });
            }
        }
    }
}