using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public struct SampleData
{
    public readonly Vector2i i;

    public readonly Rect cell;
    public readonly HeightData data;
    
    public SampleData(Matrix<float> points, Vector2i i,  Rect cell)
    {
        this.i = i;
        this.cell = cell;
        float x0y0 = points[this.i.x, i.y];
        float x0y1 = points[this.i.x, i.y + 1];
        float x1y0 = points[this.i.x + 1, i.y];
        float x1y1 = points[this.i.x + 1, i.y + 1];

        this.data = new HeightData(x0y0, x0y1, x1y0, x1y1);
    }
}


