using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.Assertions;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.Utils;

namespace Votyra.Core.Raycasting
{
    public sealed class Image2fRaycaster : BaseRaycaster
    {
        private readonly IImage2fProvider _image2FProvider;
        private IImage2f _image;

        public Image2fRaycaster(IImage2fProvider image2FProvider, ITerrainVertexPostProcessor terrainVertexPostProcessor = null)
        :base(terrainVertexPostProcessor)
        {
            _image2FProvider = image2FProvider;
        }

        public override Vector2f? Raycast(Ray3f cameraRay)
        {
            _image = _image2FProvider.CreateImage();
            (_image as IInitializableImage)?.StartUsing();

            var result = base.Raycast(cameraRay);
            
            (_image as IInitializableImage)?.FinishUsing();
            _image = null;

            return result;
        }

        protected override float GetValue(Vector2f pos) => GetLinearInterpolatedValue(_image,pos);

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


      
    }
}