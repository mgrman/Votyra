using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SimpleImageSampler : MonoBehaviour,IImageSampler
{
   public void Sample(MatrixWithOffset<int> result,IImage2i image,Rect bounds,float time)
    {
        Vector2i pointCount = result.size;

        Vector2 step = bounds.size.DivideBy(pointCount-1);

        for (int ix = -result.offset.x; ix < pointCount.x; ix++)
        {
            float x = bounds.xMin + ix * step.x;
            for (int iy = -result.offset.y; iy < pointCount.y; iy++)
            {
                float y = bounds.yMin + iy * step.y;
                Vector2 pos = new Vector2(x, y);

                result[ix, iy] = image.Sample(pos,time);
            }
        }
        
    }
}

