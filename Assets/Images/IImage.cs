using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface IImage
{
    bool IsChanged( float time);

    Range2 RangeZ { get; }

    float Sample(Vector2 point, float time);
}
