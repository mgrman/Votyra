using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface IImage2i
{
    bool IsAnimated { get; }

    Range2i RangeZ { get; }

    int Sample(Vector2i point, float time);
}
