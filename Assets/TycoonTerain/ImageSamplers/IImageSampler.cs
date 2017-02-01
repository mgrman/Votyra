using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface IImageSampler 
{  
    void Sample(MatrixWithOffset<int> result, IImage2i image, Rect bounds,float time);
}

