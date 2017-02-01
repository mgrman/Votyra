using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class RoundImage : IImage2i
{
    private readonly IImage2 _image;

    public RoundImage(IImage2 image)
    {
        _image = image;
    }

    public bool IsAnimated
    {
        get
        {
            return _image.IsAnimated;
        }
    }

    public Range2i RangeZ { get { return new Range2i(_image.RangeZ); } }

    public int Sample(Vector2 point, float time)
    {
        return Mathf.FloorToInt(_image.Sample(point, time));
    }
}

