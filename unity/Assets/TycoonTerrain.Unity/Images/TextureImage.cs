using TycoonTerrain.Common.Models;
using TycoonTerrain.Common.Utils;
using UnityEngine;

namespace TycoonTerrain.Images
{
    public class TextureImage : IImage2i
    {
        public Texture2D Texture { get; private set; }
        public Range2i RangeZ { get; private set; }
                
        public TextureImage(Range2i rangeZ, Texture2D texture)
        {
            RangeZ = rangeZ;
            Texture = texture;
        }
        
        public bool IsAnimated
        {
            get
            {
                return false;
            }
        }

        public int Sample(Vector2i point, float time)
        {
            return RangeZ.min + (int)(RangeZ.Size * Texture.GetPixel(point.x, point.y).grayscale);
        }
    }
}