using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface IImage2
{
    bool IsAnimated { get; }

    Range2 RangeZ { get; }

    float Sample(Vector2 point, float time);
}
