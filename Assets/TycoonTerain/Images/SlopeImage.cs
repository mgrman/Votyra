using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SlopeImage : MonoBehaviour, IImage
{
    public float Scale = 1;
    [Range(0, 360)]
    public float Angle = 0;

    public float MinZ = 0;
    public float MaxZ = 1;

    public Range2 RangeZ { get { return new Range2(MinZ, MaxZ); } }

    public bool IsChanged( float time)
    {
        return true;
    }

    public float Sample(Vector2 point, float time)
    {
        float sinAngle = -Mathf.Sin(Mathf.Deg2Rad * Angle);
        float cosAngle = Mathf.Cos(Mathf.Deg2Rad * Angle);
        Vector2 direction = new Vector2(cosAngle, sinAngle);


        return Mathf.Clamp(Vector2.Dot(point, direction.normalized) * direction.magnitude * Scale, MinZ, MaxZ);

    }
}

