using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface ISampler
{
    void Sample(MatrixWithOffset<float> result, IImage image, Rect bounds,float time);
}

