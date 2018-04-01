using System.Linq;
using UnityEngine;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Images
{
    public class InitialStateSetter3b
    {
        public InitialStateSetter3b(IEditableImage3b editableImage, IInitialImageConfig initialImageConfig, IImageConfig imageConfig, IImageSampler3 sampler, [Inject(Id = "root")]GameObject root)
        {
            if (editableImage == null)
                return;
            if (initialImageConfig.InitialData is Texture2D)
            {
                FillInitialState(editableImage, initialImageConfig.InitialData as Texture2D, initialImageConfig.InitialDataScale.Z, imageConfig.ImageSize.Z);
            }
            if (initialImageConfig.InitialData is GameObject)
            {
                var gameObject = initialImageConfig.InitialData as GameObject;
                FillInitialState(editableImage, gameObject.GetComponentsInChildren<Collider>(), initialImageConfig.InitialDataScale.Z, sampler, root);
                gameObject.SetActive(false);
            }
            if (initialImageConfig.InitialData is Collider)
            {
                var collider = initialImageConfig.InitialData as Collider;
                FillInitialState(editableImage, new[] { collider }, initialImageConfig.InitialDataScale.Z, sampler, root);
                collider.enabled = false;
            }
            if (initialImageConfig.InitialData is IMatrix2<float>)
            {
                FillInitialState(editableImage, initialImageConfig.InitialData as IMatrix2<float>, initialImageConfig.InitialDataScale.Z, imageConfig.ImageSize.Z);
            }
            if (initialImageConfig.InitialData is IMatrix3<float>)
            {
                FillInitialState(editableImage, initialImageConfig.InitialData as IMatrix3<float>, initialImageConfig.InitialDataScale.Z);
            }
        }

        private static void FillInitialState(IEditableImage3b editableImage, Texture2D texture, float scale, int fallbackMaxZ)
        {
            using (var imageAccessor = editableImage.RequestAccess(Rect3i.All))
            {
                Rect3i matrixAreaToFill;
                if (imageAccessor.Area == Rect3i.All)
                {
                    matrixAreaToFill = new Vector3i(texture.width, texture.height, fallbackMaxZ).ToRect3i();
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
                        var value = texture.GetPixelBilinear((float)x / matrixSizeX, (float)y / matrixSizeY).grayscale * scale;
                        for (int z = matrixAreaToFill.Min.Z; z < matrixAreaToFill.Max.Z; z++)
                        {
                            var pos = new Vector3i(x, y, z);
                            imageAccessor[pos] = value - z > 0;
                        }
                    }
                }
            }
        }

        private static void FillInitialState(IEditableImage3b editableImage, Collider[] colliders, float scale, IImageSampler3 sampler, GameObject root)
        {
            var bounds = colliders.Select(o => o.bounds)
                .Select(o => Rect3f.FromMinAndSize(o.min.ToVector3f(), o.size.ToVector3f()))
                .DefaultIfEmpty(Rect3f.zero)
                .Aggregate((a, b) => a.Encapsulate(b));

            float maxSize = bounds.DiagonalLength;

            using (var imageAccessor = editableImage.RequestAccess(Rect3i.All))
            {
                var area = imageAccessor.Area;
                for (int ix = area.Min.X; ix < area.Max.X; ix++)
                {
                    for (int iy = area.Min.Y; iy < area.Max.Y; iy++)
                    {
                        for (int iz = area.Min.Z; iz < area.Max.Z; iz++)
                        {
                            var i = new Vector3i(ix, iy, iz);
                            var localPos = sampler.ImageToWorld(i);
                            var worldPos = root.transform.TransformPoint(new Vector3(localPos.X, localPos.Y, localPos.Z));

                            imageAccessor[i] = colliders
                                .Select(collider =>
                                {
                                    return IsInside(collider, worldPos, maxSize);
                                })
                                .Any(o => o);
                        }
                    }
                }
            }
        }

        private static bool IsInside(Collider collider, Vector3 initialPoint, float maxSize)
        {
            var outsidePoint = initialPoint + new Vector3(0, maxSize, 0) * 2;

            int counter = 0;
            bool isHit = false;
            var point = initialPoint;
            do
            {
                RaycastHit hit;
                isHit = collider.Raycast(new Ray(point, Vector3.up), out hit, maxSize * 3);
                if (isHit)
                {
                    counter++;
                    point = hit.point + Vector3.up * (maxSize / 1000f);
                }
            }
            while (isHit);

            isHit = false;
            point = outsidePoint;
            do
            {
                RaycastHit hit;
                isHit = collider.Raycast(new Ray(point, Vector3.down), out hit, maxSize * 3);
                if (hit.point.y < initialPoint.y)
                {
                    break;
                }
                if (isHit)
                {
                    counter++;
                    point = hit.point + Vector3.down * (maxSize / 1000f);
                }
            }
            while (isHit);
            return counter % 2 == 1;
        }

        private static void FillInitialState(IEditableImage3b editableImage, IMatrix2<float> texture, float scale, int fallbackMaxZ)
        {
            using (var imageAccessor = editableImage.RequestAccess(Rect3i.All))
            {
                Rect3i matrixAreaToFill;
                if (imageAccessor.Area == Rect3i.All)
                {
                    matrixAreaToFill = new Vector3i(texture.Size.X, texture.Size.Y, fallbackMaxZ).ToRect3i();
                }
                else
                {
                    matrixAreaToFill = imageAccessor.Area;
                }

                for (int ix = matrixAreaToFill.Min.X; ix < matrixAreaToFill.Max.X; ix++)
                {
                    for (int iy = matrixAreaToFill.Min.Y; iy < matrixAreaToFill.Max.Y; iy++)
                    {
                        var i = new Vector2i(ix, iy);
                        var value = texture[i] * scale;

                        for (int z = matrixAreaToFill.Min.Z; z < matrixAreaToFill.Max.Z; z++)
                        {
                            var pos = new Vector3i(ix, iy, z);
                            imageAccessor[pos] = value - z > 0;
                        }
                    }
                }
            }
        }

        private static void FillInitialState(IEditableImage3b editableImage, IMatrix3<float> texture, float scale)
        {
            using (var imageAccessor = editableImage.RequestAccess(Rect3i.All))
            {
                Rect3i matrixAreaToFill;
                if (imageAccessor.Area == Rect3i.All)
                {
                    matrixAreaToFill = texture.Size.ToRect3i();
                }
                else
                {
                    matrixAreaToFill = imageAccessor.Area;
                }

                for (int ix = matrixAreaToFill.Min.X; ix < matrixAreaToFill.Max.X; ix++)
                {
                    for (int iy = matrixAreaToFill.Min.Y; iy < matrixAreaToFill.Max.Y; iy++)
                    {
                        for (int iz = matrixAreaToFill.Min.Z; iz < matrixAreaToFill.Max.Z; iz++)
                        {
                            var i = new Vector3i(ix, iy, iz);

                            imageAccessor[i] = texture[i] * scale > 0;
                        }
                    }
                }
            }
        }
    }
}