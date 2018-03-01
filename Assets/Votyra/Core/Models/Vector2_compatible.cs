using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Votyra.Core.Models
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Vector2_compatible
    {
        [FieldOffset(0)]
        public Vector2 Unity;

        [FieldOffset(0)]
        public readonly float X;

        [FieldOffset(sizeof(float))]
        public readonly float Y;

        public Vector2_compatible(float x, float y)
        {
            Unity = Vector2.zero;
            X = x;
            Y = y;
        }
    }
}