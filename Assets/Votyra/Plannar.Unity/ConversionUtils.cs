using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Plannar.Images;

namespace Votyra.Plannar
{
    public static class ConversionUtils
    {
        public static Vector2f ToVector2f(this UnityEngine.Vector2 vector)
        {
            return new Vector2f(vector.x, vector.y);
        }

        public static Vector3f ToVector3f(this UnityEngine.Vector3 vector)
        {
            return new Vector3f(vector.x, vector.y, vector.z);
        }

        public static Vector3f[] ToVector3f(this PooledArrayContainer<UnityEngine.Vector3> planesUnity)
        {
            Vector3f[] planes = new Vector3f[planesUnity.Count];
            for (int i = 0; i < planes.Length; i++)
            {
                planes[i] = planesUnity.Array[i].ToVector3f();
            }
            return planes;
        }

        public static Plane3f ToPlane3f(this UnityEngine.Plane plane)
        {
            return new Plane3f(plane.normal.ToVector3f(), plane.distance);
        }

        public static Plane3f[] ToPlane3f(this PooledArrayContainer<UnityEngine.Plane> planesUnity)
        {
            Plane3f[] planes = new Plane3f[planesUnity.Count];
            for (int i = 0; i < planes.Length; i++)
            {
                planes[i] = planesUnity[i].ToPlane3f();
            }
            return planes;
        }

        public static UnityEngine.Vector2 ToVector2(this Vector2f vector)
        {
            return new UnityEngine.Vector2(vector.x, vector.y);
        }

        public static UnityEngine.Vector3 ToVector3(this Vector3f vector)
        {
            return new UnityEngine.Vector3(vector.x, vector.y, vector.z);
        }

        public static UnityEngine.Bounds ToBounds(this Rect3f bounds)
        {
            return new UnityEngine.Bounds(bounds.center.ToVector3(), bounds.size.ToVector3());
        }

        public static List<UnityEngine.Vector3> ToVector3(this List<Vector3f> vector)
        {
            throw new NotSupportedException();
            return vector.Select(o => o.ToVector3()).ToList();
        }

        public static UnityEngine.Vector3[] ToVector3(this Vector3f[] vector)
        {
            var union = new UnionVector3();
            union.Votyra = vector;
            return union.Unity;
            //return vector.Select(o => o.ToVector3()).ToArray();
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct UnionVector3
        {
            [FieldOffset(0)] public UnityEngine.Vector3[] Unity;
            [FieldOffset(0)] public Vector3f[] Votyra;
        }

        public static List<UnityEngine.Vector2> ToVector2(this List<Vector2f> vector)
        {
            throw new NotSupportedException();
            return vector.Select(o => o.ToVector2()).ToList();
        }

        public static UnityEngine.Vector2[] ToVector2(this Vector2f[] vector)
        {
            var union = new UnionVector2();
            union.Votyra = vector;
            return union.Unity;
            //return vector.Select(o => o.ToVector2()).ToArray();
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct UnionVector2
        {
            [FieldOffset(0)] public UnityEngine.Vector2[] Unity;
            [FieldOffset(0)] public Vector2f[] Votyra;
        }


        public static Matrix4x4f ToMatrix4x4f(this UnityEngine.Matrix4x4 mat)
        {
            return new Matrix4x4f(mat.m00, mat.m01, mat.m02, mat.m03, mat.m10, mat.m11, mat.m12, mat.m13, mat.m20, mat.m21, mat.m22, mat.m23, mat.m30, mat.m31, mat.m32, mat.m33);
        }

        public static UnityEngine.Matrix4x4 ToMatrix4x4(this Matrix4x4f mat)
        {
            var mat2 = new UnityEngine.Matrix4x4();

            mat2.m00 = mat.m00;
            mat2.m10 = mat.m10;
            mat2.m20 = mat.m20;
            mat2.m30 = mat.m30;
            mat2.m01 = mat.m01;
            mat2.m11 = mat.m11;
            mat2.m21 = mat.m21;
            mat2.m31 = mat.m31;
            mat2.m02 = mat.m02;
            mat2.m12 = mat.m12;
            mat2.m22 = mat.m22;
            mat2.m32 = mat.m32;
            mat2.m03 = mat.m03;
            mat2.m13 = mat.m13;
            mat2.m23 = mat.m23;
            mat2.m33 = mat.m33;
            return mat2;
        }
    }
}