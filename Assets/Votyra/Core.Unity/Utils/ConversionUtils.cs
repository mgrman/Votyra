using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.Utils
{
    public static unsafe class ConversionUtils
    {
        public static Vector2f ToVector2f(this Vector2 vector) => new Vector2f(vector.x, vector.y);

        public static Vector3f ToVector3f(this Vector3 vector) => new Vector3f(vector.x, vector.y, vector.z);

        public static Plane3f ToPlane3f(this Plane plane) => new Plane3f(plane.normal.ToVector3f(), plane.distance);

        public static Plane3f[] ToPlane3f(this Plane[] planesUnity)
        {
            var union = new UnionPlane();
            union.From = planesUnity;
            var res = union.To;
            Assert.AreEqual(planesUnity.Length, res.Length, $"{nameof(ToPlane3f)} conversion failed!");
            return res;
        }

        public static Vector2 ToVector2(this Vector2f vector) => *(Vector2*) &vector;

        public static Vector3 ToVector3(this Vector3f vector) => *(Vector3*) &vector;

        public static Bounds ToBounds(this Area3f bounds) => new Bounds(bounds.Center.ToVector3(), bounds.Size.ToVector3());

        public static Vector3[] ToVector3(this Vector3f[] vector)
        {
            var union = new UnionVector3();
            union.From = vector;
            var res = union.To;
            Assert.AreEqual(vector.Length, res.Length, $"{nameof(ToVector3)} conversion failed!");
            return res;
        }

        public static Vector2[] ToVector2(this Vector2f[] vector)
        {
            var union = new UnionVector2();
            union.From = vector;
            var res = union.To;
            Assert.AreEqual(vector.Length, res.Length, $"{nameof(ToVector2)}  conversion failed!");
            return res;
        }

        public static List<Vector2> ToVector2List(this List<Vector2f> vector) => vector.ConvertListOfMatchingStructs(ToVector2);

        public static List<Vector3> ToVector3List(this List<Vector3f> vector) => vector.ConvertListOfMatchingStructs(ToVector3);

        public static Matrix4x4f ToMatrix4x4f(this Matrix4x4 mat) => new Matrix4x4f(mat.m00, mat.m01, mat.m02, mat.m03, mat.m10, mat.m11, mat.m12, mat.m13, mat.m20, mat.m21, mat.m22, mat.m23, mat.m30, mat.m31, mat.m32, mat.m33);

        public static Matrix4x4 ToMatrix4x4(this Matrix4x4f mat)
        {
            var mat2 = new Matrix4x4();

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
        
        //
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