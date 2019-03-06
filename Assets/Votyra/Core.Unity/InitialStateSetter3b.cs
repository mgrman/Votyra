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
        public InitialStateSetter3b(IEditableImage3b editableImage, IInitialImageConfig initialImageConfig, IImageConfig imageConfig, IImageSampler3 sampler, [Inject(Id = "root")] GameObject root)
        {
            if (editableImage == null)
                return;
            if (initialImageConfig.InitialData is Texture2D)
                FillInitialState(editableImage, initialImageConfig.InitialData as Texture2D, initialImageConfig.InitialDataScale.Z, imageConfig.ImageSize.Z);
            if (initialImageConfig.InitialData is GameObject)
            {
                var gameObject = initialImageConfig.InitialData as GameObject;
                FillInitialState(editableImage, gameObject.GetComponentsInChildren<Collider>(), initialImageConfig.InitialDataScale.Z, sampler, root);
                gameObject.SetActive(false);
            }

            if (initialImageConfig.InitialData is Collider)
            {
                var collider = initialImageConfig.InitialData as Collider;
                FillInitialState(editableImage, new[] {collider}, initialImageConfig.InitialDataScale.Z, sampler, root);
                collider.enabled = false;
            }

            if (initialImageConfig.InitialData is IMatrix2<int?>)
                FillInitialState(editableImage, initialImageConfig.InitialData as IMatrix2<int?>, initialImageConfig.InitialDataScale.Z, imageConfig.ImageSize.Z);
            if (initialImageConfig.InitialData is IMatrix3<bool>)
                FillInitialState(editableImage, initialImageConfig.InitialData as IMatrix3<bool>, initialImageConfig.InitialDataScale.Z);
        }

        private static void FillInitialState(IEditableImage3b editableImage, Texture2D texture, float scale, int fallbackMaxZ)
        {
            using (var imageAccessor = editableImage.RequestAccess(Range3i.All))
            {
                Range3i matrixAreaToFill;
                if (imageAccessor.Area == Range3i.All)
                    matrixAreaToFill = new Vector3i(texture.width, texture.height, fallbackMaxZ).ToRange3i();
                else
                    matrixAreaToFill = imageAccessor.Area;
                var matrixSizeX = matrixAreaToFill.Size.X;
                var matrixSizeY = matrixAreaToFill.Size.Y;

                var min = matrixAreaToFill.Min;
                for (var ix = 0; ix < matrixAreaToFill.Size.X; ix++)
                {
                    for (var iy = 0; iy < matrixAreaToFill.Size.Y; iy++)
                    {
                        for (var iz = 0; iz < matrixAreaToFill.Size.Z; iz++)
                        {
                            var i = new Vector3i(ix, iy, iz) + min;
                            var value = texture.GetPixelBilinear((float) i.X / matrixSizeX, (float) i.Y / matrixSizeY)
                                .grayscale * scale;
                            imageAccessor[i] = value - i.Z > 0;
                        }
                    }
                }
            }
        }

        private static void FillInitialState(IEditableImage3b editableImage, Collider[] colliders, float scale, IImageSampler3 sampler, GameObject root)
        {
            var bounds = colliders.Select(o => o.bounds)
                .Select(o => Area3f.FromMinAndSize(o.min.ToVector3f(), o.size.ToVector3f()))
                .DefaultIfEmpty(Area3f.Zero)
                .Aggregate((a, b) => a.Encapsulate(b));

            var maxSize = bounds.DiagonalLength;

            using (var imageAccessor = editableImage.RequestAccess(Range3i.All))
            {
                var area = imageAccessor.Area;

                var min = area.Min;
                for (var ix = 0; ix < area.Size.X; ix++)
                {
                    for (var iy = 0; iy < area.Size.Y; iy++)
                    {
                        for (var iz = 0; iz < area.Size.Z; iz++)
                        {
                            var i = new Vector3i(ix, iy, iz) + min;
                            var localPos = sampler.ImageToWorld(i);
                            var worldPos = root.transform.TransformPoint(new Vector3(localPos.X, localPos.Y, localPos.Z));

                            imageAccessor[i] = colliders.Select(collider =>
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

            var counter = 0;
            var isHit = false;
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
            } while (isHit);

            isHit = false;
            point = outsidePoint;
            do
            {
                RaycastHit hit;
                isHit = collider.Raycast(new Ray(point, Vector3.down), out hit, maxSize * 3);
                if (hit.point.y < initialPoint.y)
                    break;
                if (isHit)
                {
                    counter++;
                    point = hit.point + Vector3.down * (maxSize / 1000f);
                }
            } while (isHit);

            return counter % 2 == 1;
        }

        private static void FillInitialState(IEditableImage3b editableImage, IMatrix2<int?> texture, float scale, int fallbackMaxZ)
        {
            using (var imageAccessor = editableImage.RequestAccess(Range3i.All))
            {
                Range3i matrixAreaToFill;
                if (imageAccessor.Area == Range3i.All)
                    matrixAreaToFill = new Vector3i(texture.Size.X, texture.Size.Y, fallbackMaxZ).ToRange3i();
                else
                    matrixAreaToFill = imageAccessor.Area;
                var min = matrixAreaToFill.Min;
                for (var ix = 0; ix < matrixAreaToFill.Size.X; ix++)
                {
                    for (var iy = 0; iy < matrixAreaToFill.Size.Y; iy++)
                    {
                        for (var iz = 0; iz < matrixAreaToFill.Size.Z; iz++)
                        {
                            var i = new Vector3i(ix, iy, iz) + min;
                            var value = texture[i.XY()] * scale;
                            imageAccessor[i] = value - i.Z > 0;
                        }
                    }
                }
            }
        }

        private static void FillInitialState(IEditableImage3b editableImage, IMatrix3<bool> texture, float scale)
        {
            using (var imageAccessor = editableImage.RequestAccess(Range3i.All))
            {
                Range3i matrixAreaToFill;
                if (imageAccessor.Area == Range3i.All)
                    matrixAreaToFill = texture.Size.ToRange3i();
                else
                    matrixAreaToFill = imageAccessor.Area;

                var min = matrixAreaToFill.Min;
                for (var ix = 0; ix < matrixAreaToFill.Size.X; ix++)
                {
                    for (var iy = 0; iy < matrixAreaToFill.Size.Y; iy++)
                    {
                        for (var iz = 0; iz < matrixAreaToFill.Size.Z; iz++)
                        {
                            var i = new Vector3i(ix, iy, iz) + min;
                            imageAccessor[i] = texture[i];
                        }
                    }
                }
            }
        }
    }
}