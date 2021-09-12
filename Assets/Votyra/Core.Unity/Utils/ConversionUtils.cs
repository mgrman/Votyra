using UnityEngine;
using Votyra.Core.Models;

namespace Votyra.Core.Unity.Utils
{
    public static class ConversionUtils
    {
        public static Vector2f ToVector2f(this Vector2 vector) => new Vector2f(vector.x, vector.y);

        public static Vector3f ToVector3f(this Vector3 vector) => new Vector3f(vector.x, vector.y, vector.z);

        public static void ToVector3f(this Vector3[] planesUnity, Vector3f[] votyraArray)
        {
            for (var i = 0; i < planesUnity.Length; i++)
            {
                votyraArray[i] = planesUnity[i]
                    .ToVector3f();
            }

        }

        public static Plane3f ToPlane3f(this Plane plane) => new Plane3f(plane.normal.ToVector3f(), plane.distance);

        public static void ToPlane3f(this Plane[] planesUnity,Plane3f[] votyraArray )
        {
            var unityArray = planesUnity;


            for (var i = 0; i < unityArray.Length; i++)
            {
                votyraArray[i] = unityArray[i]
                    .ToPlane3f();
            }
        }

        public static Vector2 ToVector2(this Vector2f vector) => new Vector2(vector.X, vector.Y);

        public static Vector3 ToVector3(this Vector3f vector) => new Vector3(vector.X, vector.Y, vector.Z);

        public static Bounds ToBounds(this Area3f bounds) => new Bounds(bounds.Center.ToVector3(), bounds.Size.ToVector3());

        public static Matrix4x4f ToMatrix4x4f(this Matrix4x4 mat) => new Matrix4x4f(mat.m00, mat.m01, mat.m02, mat.m03, mat.m10, mat.m11, mat.m12, mat.m13, mat.m20, mat.m21, mat.m22, mat.m23, mat.m30, mat.m31, mat.m32, mat.m33);

        public static Matrix4x4 ToMatrix4x4(this Matrix4x4f mat)
        {
            var mat2 = new Matrix4x4();

            mat2.m00 = mat.M00;
            mat2.m10 = mat.M10;
            mat2.m20 = mat.M20;
            mat2.m30 = mat.M30;
            mat2.m01 = mat.M01;
            mat2.m11 = mat.M11;
            mat2.m21 = mat.M21;
            mat2.m31 = mat.M31;
            mat2.m02 = mat.M02;
            mat2.m12 = mat.M12;
            mat2.m22 = mat.M22;
            mat2.m32 = mat.M32;
            mat2.m03 = mat.M03;
            mat2.m13 = mat.M13;
            mat2.m23 = mat.M23;
            mat2.m33 = mat.M33;
            return mat2;
        }
    }
}