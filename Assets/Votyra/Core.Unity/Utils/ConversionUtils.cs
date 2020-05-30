using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using Votyra.Core.Models;

namespace Votyra.Core.Utils
{
    public static unsafe class ConversionUtils
    {
        public static Vector2f ToVector2f(this Vector2 vector) => new Vector2f(vector.x, vector.y);

        public static Vector3f ToVector3F(this Vector3 vector) => new Vector3f(vector.x, vector.y, vector.z);

        public static Plane3f ToPlane3F(this Plane plane) => new Plane3f(plane.normal.ToVector3F(), plane.distance);

        public static Plane3f[] ToPlane3F(this Plane[] planesUnity)
        {
            var union = new UnionPlane
            {
                From = planesUnity,
            };
            var res = union.To;
            Assert.AreEqual(planesUnity.Length, res.Length, $"{nameof(ToPlane3F)} conversion failed!");
            return res;
        }

        public static Vector2 ToVector2(this Vector2f vector) => *(Vector2*)&vector;

        public static Vector3 ToVector3(this Vector3f vector) => *(Vector3*)&vector;

        public static Bounds ToBounds(this Area3f bounds) => new Bounds(bounds.Center.ToVector3(), bounds.Size.ToVector3());

        public static Vector3[] ToVector3(this Vector3f[] vector)
        {
            var union = new UnionVector3
            {
                From = vector,
            };
            var res = union.To;
            Assert.AreEqual(vector.Length, res.Length, $"{nameof(ToVector3)} conversion failed!");
            return res;
        }

        public static Vector2[] ToVector2(this Vector2f[] vector)
        {
            var union = new UnionVector2
            {
                From = vector,
            };
            var res = union.To;
            Assert.AreEqual(vector.Length, res.Length, $"{nameof(ToVector2)}  conversion failed!");
            return res;
        }

        public static List<Vector2> ToVector2List(this List<Vector2f> vector) => vector.ConvertListOfMatchingStructs(ToVector2);

        public static List<Vector3> ToVector3List(this List<Vector3f> vector) => vector.ConvertListOfMatchingStructs(ToVector3);

        public static Matrix4X4F ToMatrix4X4F(this Matrix4x4 mat) => new Matrix4X4F(mat.m00, mat.m01, mat.m02, mat.m03, mat.m10, mat.m11, mat.m12, mat.m13, mat.m20, mat.m21, mat.m22, mat.m23, mat.m30, mat.m31, mat.m32, mat.m33);

        public static Matrix4x4 ToMatrix4X4(this Matrix4X4F mat)
        {
            var mat2 = new Matrix4x4
            {
                m00 = mat.M00,
                m10 = mat.M10,
                m20 = mat.M20,
                m30 = mat.M30,
                m01 = mat.M01,
                m11 = mat.M11,
                m21 = mat.M21,
                m31 = mat.M31,
                m02 = mat.M02,
                m12 = mat.M12,
                m22 = mat.M22,
                m32 = mat.M32,
                m03 = mat.M03,
                m13 = mat.M13,
                m23 = mat.M23,
                m33 = mat.M33,
            };
            return mat2;
        }

        private static List<TResult> ConvertListOfMatchingStructs<TSource, TResult>(this List<TSource> source, Func<TSource[], TResult[]> convert)
        {
            var target = new List<TResult>();
            var targetItemsSet = ListInternals<TResult>.ItemsSet;
            var sourceItemsGet = ListInternals<TSource>.ItemsGet;
            var targetSizeSet = ListInternals<TResult>.SizeSet;

            var sourceItemsValue = sourceItemsGet(source);
            var targetItems = convert(sourceItemsValue);

            targetItemsSet(target, targetItems);
            // targetItemsSet.SetValue(target, targetItems, BindingFlags.SetField, new ArrayKeepBinder<TSource, TResult>(), null);
            targetSizeSet(target, source.Count);
            return target;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct UnionVector3
        {
            [FieldOffset(0)]
            public Vector3f[] From;

            [FieldOffset(0)]
            public readonly Vector3[] To;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct UnionVector2
        {
            [FieldOffset(0)]
            public Vector2f[] From;

            [FieldOffset(0)]
            public readonly Vector2[] To;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct UnionPlane
        {
            [FieldOffset(0)]
            public Plane[] From;

            [FieldOffset(0)]
            public readonly Plane3f[] To;
        }

        //        [StructLayout(LayoutKind.Explicit)]
        //        private struct UnionVector3
        //        {
        //            [FieldOffset(0)] public UnityEngine.Vector3[] Unity;
        //            [FieldOffset(0)] public Vector3f[] Votyra;
        //        }
        //
        //        [StructLayout(LayoutKind.Explicit)]
        //        private struct UnionVector2
        //        {
        //            [FieldOffset(0)] public UnityEngine.Vector2[] Unity;
        //            [FieldOffset(0)] public Vector2f[] Votyra;
        //        }
        //
        //        [StructLayout(LayoutKind.Explicit)]
        //        private struct UnionPlane
        //        {
        //            [FieldOffset(0)] public UnityEngine.Plane[] Unity;
        //            [FieldOffset(0)] public Plane3f[] Votyra;
        //        }

        private static class ListInternals<T>
        {
            public static readonly Func<List<T>, T[]> ItemsGet;
            public static readonly Action<List<T>, T[]> ItemsSet;
            public static readonly Func<List<T>, int> SizeGet;
            public static readonly Action<List<T>, int> SizeSet;

            static ListInternals()
            {
#if ENABLE_IL2CPP
                var itemsField = typeof(List<T>).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic);
                ItemsGet = o => (T[]) itemsField.GetValue(o);
                ItemsSet = (o, value) => itemsField.SetValue(o, value);

                var sizeField = typeof(List<T>).GetField("_size", BindingFlags.Instance | BindingFlags.NonPublic);
                SizeGet = o => (int) itemsField.GetValue(o);
                SizeSet = (o, value) => itemsField.SetValue(o, value);

#else
                var itemsField = typeof(List<T>).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic);
                ItemsGet = CreateGetFieldDelegate<List<T>, T[]>(itemsField);
                ItemsSet = CreateSetFieldDelegate<List<T>, T[]>(itemsField);

                var sizeField = typeof(List<T>).GetField("_size", BindingFlags.Instance | BindingFlags.NonPublic);
                SizeGet = CreateGetFieldDelegate<List<T>, int>(sizeField);
                SizeSet = CreateSetFieldDelegate<List<T>, int>(sizeField);
#endif
            }

            private static Func<TOwner, TValue> CreateGetFieldDelegate<TOwner, TValue>(FieldInfo fieldInfo)
            {
                var ownerParameter = Expression.Parameter(typeof(TOwner));

                var fieldExpression = Expression.Field(Expression.Convert(ownerParameter, typeof(TOwner)), fieldInfo);

                return Expression.Lambda<Func<TOwner, TValue>>(Expression.Convert(fieldExpression, typeof(TValue)), ownerParameter)
                    .Compile();
            }

            private static Action<TOwner, TValue> CreateSetFieldDelegate<TOwner, TValue>(FieldInfo fieldInfo)
            {
                var ownerParameter = Expression.Parameter(typeof(TOwner));
                var fieldParameter = Expression.Parameter(typeof(TValue));

                var fieldExpression = Expression.Field(Expression.Convert(ownerParameter, typeof(TOwner)), fieldInfo);

                return Expression.Lambda<Action<TOwner, TValue>>(Expression.Assign(fieldExpression, Expression.Convert(fieldParameter, fieldInfo.FieldType)), ownerParameter, fieldParameter)
                    .Compile();
            }
        }
    }
}