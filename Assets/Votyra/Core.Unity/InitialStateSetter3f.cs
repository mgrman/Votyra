using System.Linq;
using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Images
{
    public class InitialStateSetter3f
    {
        public InitialStateSetter3f(IEditableImage3f editableImage, IInitialImageConfig initialImageConfig, IImageConfig imageConfig, IImageSampler3b sampler, [Inject(Id = "root")]GameObject root)
        {
            if (editableImage == null)
                return;
            if (initialImageConfig.InitialData is Texture2D)
            {
                FillInitialState(editableImage, initialImageConfig.InitialData as Texture2D, initialImageConfig.InitialDataScale.z, imageConfig.ImageSize.z);
            }
            if (initialImageConfig.InitialData is GameObject)
            {
                FillInitialState(editableImage, (initialImageConfig.InitialData as GameObject).GetComponentsInChildren<Collider>(), initialImageConfig.InitialDataScale.z, sampler, root);
            }
            if (initialImageConfig.InitialData is Collider)
            {
                FillInitialState(editableImage, new[] { initialImageConfig.InitialData as Collider }, initialImageConfig.InitialDataScale.z, sampler, root);
            }
            if (initialImageConfig.InitialData is IMatrix2<float>)
            {
                FillInitialState(editableImage, initialImageConfig.InitialData as IMatrix2<float>, initialImageConfig.InitialDataScale.z, imageConfig.ImageSize.z);
            }
            if (initialImageConfig.InitialData is IMatrix3<float>)
            {
                FillInitialState(editableImage, initialImageConfig.InitialData as IMatrix3<float>, initialImageConfig.InitialDataScale.z);
            }
        }

        private static void FillInitialState(IEditableImage3f editableImage, Texture2D texture, float scale, int fallbackMaxZ)
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

                var matrixSizeX = matrixAreaToFill.size.x;
                var matrixSizeY = matrixAreaToFill.size.y;

                for (int x = matrixAreaToFill.xMin; x < matrixAreaToFill.xMax; x++)
                {
                    for (int y = matrixAreaToFill.yMin; y < matrixAreaToFill.yMax; y++)
                    {
                        var value = texture.GetPixelBilinear((float)x / matrixSizeX, (float)y / matrixSizeY).grayscale * scale;
                        for (int z = matrixAreaToFill.zMin; z < matrixAreaToFill.zMax; z++)
                        {
                            var pos = new Vector3i(x, y, z);
                            imageAccessor[pos] = value - z;
                        }
                    }
                }
            }
        }

        private static void FillInitialState(IEditableImage3f editableImage, Collider[] colliders, float scale, IImageSampler3b sampler, GameObject root)
        {
            var bounds = colliders.Select(o => o.bounds)
                .Select(o => new Rect3f(o.center.ToVector3f(), o.size.ToVector3f()))
                .DefaultIfEmpty(Rect3f.zero)
                .Aggregate((a, b) => a.Encapsulate(b));

            float maxSize = bounds.DiagonalLength;

            using (var imageAccessor = editableImage.RequestAccess(Rect3i.All))
            {
                var area = imageAccessor.Area;
                for (int ix = area.xMin; ix < area.xMax; ix++)
                {
                    for (int iy = area.yMin; iy < area.yMax; iy++)
                    {
                        for (int iz = area.zMin; iz < area.zMax; iz++)
                        {
                            var i = new Vector3i(ix, iy, iz);
                            var localPos = sampler.ImageToWorld(i);
                            var worldPos = root.transform.TransformPoint(new Vector3(localPos.x, localPos.y, localPos.z));

                            imageAccessor[i] = colliders
                                .Select(collider =>
                                {
                                    return IsInside(collider, worldPos, maxSize) ? 1f : -1f;
                                })
                                .DefaultIfEmpty(0)
                                .Max() * scale;

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

        private static void FillInitialState(IEditableImage3f editableImage, IMatrix2<float> texture, float scale, int fallbackMaxZ)
        {
            using (var imageAccessor = editableImage.RequestAccess(Rect3i.All))
            {
                Rect3i matrixAreaToFill;
                if (imageAccessor.Area == Rect3i.All)
                {
                    matrixAreaToFill = new Vector3i(texture.size.x, texture.size.y, fallbackMaxZ).ToRect3i();
                }
                else
                {
                    matrixAreaToFill = imageAccessor.Area;
                }

                for (int x = matrixAreaToFill.xMin; x < matrixAreaToFill.xMax; x++)
                {
                    for (int y = matrixAreaToFill.yMin; y < matrixAreaToFill.yMax; y++)
                    {
                        var value = texture[x, y] * scale;

                        for (int z = matrixAreaToFill.zMin; z < matrixAreaToFill.zMax; z++)
                        {
                            var pos = new Vector3i(x, y, z);
                            imageAccessor[pos] = value - z;
                        }
                    }
                }
            }
        }

        private static void FillInitialState(IEditableImage3f editableImage, IMatrix3<float> texture, float scale)
        {
            using (var imageAccessor = editableImage.RequestAccess(Rect3i.All))
            {
                Rect3i matrixAreaToFill;
                if (imageAccessor.Area == Rect3i.All)
                {
                    matrixAreaToFill = texture.size.ToRect3i();
                }
                else
                {
                    matrixAreaToFill = imageAccessor.Area;
                }

                for (int x = matrixAreaToFill.xMin; x < matrixAreaToFill.xMax; x++)
                {
                    for (int y = matrixAreaToFill.yMin; y < matrixAreaToFill.yMax; y++)
                    {
                        for (int z = matrixAreaToFill.zMin; z < matrixAreaToFill.zMax; z++)
                        {
                            var pos = new Vector3i(x, y, z);

                            imageAccessor[pos] = texture[x, y, z] * scale;
                        }
                    }
                }
            }
        }
    }
}