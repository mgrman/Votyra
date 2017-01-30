using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class GaussianImage : MonoBehaviour, IImage
{
    public float σ = 1;
    public Vector3 scale = Vector3.one;

    public Range2 RangeZ { get { return new Range2(0, scale.z); } }

    public bool IsChanged( float time)
    {
        return true;
    }

    public float Sample(Vector2 point, float time)
    {
        float x = point.x / scale.x;
        float y = point.y / scale.y;

        float first = 1 / (2 * Mathf.PI * σ * σ);
        float power = (x * x + y * y) / (2 * σ * σ);

        return scale.z * first * Mathf.Exp(-power);
    }

}

