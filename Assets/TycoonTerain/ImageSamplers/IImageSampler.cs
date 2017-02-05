using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface IImageSampler
{
    HeightData Sample(IImage2i image, Vector2i offset, float time);
}

