using System.Runtime.CompilerServices;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core.Utils
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

        //         public static UnityEngine.Vector3[] ToVector3Array(this List<Vector3f> vector)
        //         {
        //             return vector.Select(o => o.ToVector3()).ToArray();
        // //            return vector.GetInnerArray<Vector3f>().ToVector3();
        //         }
        //
        //         public static List<UnityEngine.Vector3> ToVector3List(this List<Vector3f> vector)
        //         {
        //             return vector.Select(o => o.ToVector3()).ToList();
        // //            return vector.ConvertListOfMatchingStructs<Vector3f, UnityEngine.Vector3>(ToVector3);
        //         }

        // public static Plane3f[] ToPlane3f(this UnityEngine.Plane[] vector)
        // {
        //     return Array.ConvertAll(vector, item => item.ToPlane3f());
        //
        //     // var union = new UnionPlane();
        //     // union.Unity = vector;
        //     // var res = union.Votyra;
        //     // if (res.Length != vector.Length)
        //     // {
        //     //     throw new InvalidOperationException("ToPlane3f conversion failed!");
        //     // }
        //     // return res;
        // }

        // public static UnityEngine.Vector3[] ToVector3(this Vector3f[] vector)
        // {
        //     return Array.ConvertAll(vector, item => item.ToVector3());
        //
        //
        //     // var union = new UnionVector3();
        //     // union.Votyra = vector;
        //     // var res = union.Unity;
        //     // if (res.Length != vector.Length)
        //     // {
        //     //     throw new InvalidOperationException("ToVector3 conversion failed!");
        //     // }
        //     // return res;
        //     // //return vector.Select(o => o.ToVector3()).ToArray();
        // }

        // public static Vector3f[] ToVector3f(this UnityEngine.Vector3[] vector)
        // {
        //     return Array.ConvertAll(vector, item => item.ToVector3f());
        //
        //     // var union = new UnionVector3();
        //     // union.Unity = vector;
        //     // var res = union.Votyra;
        //     // if (res.Length != vector.Length)
        //     // {
        //     //     throw new InvalidOperationException("ToVector3f conversion failed!");
        //     // }
        //     // return res;
        //     // //return vector.Select(o => o.ToVector3()).ToArray();
        // }

        // public static UnityEngine.Vector2[] ToVector2(this Vector2f[] vector)
        // {
        //     return Array.ConvertAll(vector, item => item.ToVector2());
        //
        //     // var union = new UnionVector2();
        //     // union.Votyra = vector;
        //     // var res = union.Unity;
        //     // if (res.Length != vector.Length)
        //     // {
        //     //     throw new InvalidOperationException("ToVector2 conversion failed!");
        //     // }
        //     // return res;
        //     // //return vector.Select(o => o.ToVector2()).ToArray();
        // }

        //         public static UnityEngine.Vector2[] ToVector2Array(this List<Vector2f> vector)
        //         {
        //             return vector.Select(o => o.ToVector2()).ToArray();
        // //            return vector.GetInnerArray<Vector2f>().ToVector2();
        //         }
        //
        //         public static List<UnityEngine.Vector2> ToVector2List(this List<Vector2f> vector)
        //         {
        //             return vector.Select(o => o.ToVector2()).ToList();
        // //            return vector.ConvertListOfMatchingStructs<Vector2f, UnityEngine.Vector2>(ToVector2);
        //         }

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

        //        private static T[] GetInnerArray<T>(this List<T> source)
        //        {
        //            source.TrimExcess();
        //            var itemsGet = ListInternals<T>.ItemsGet;
        //            var res = itemsGet(source);
        //            if (res.Length != source.Count)
        //            {
        //                throw new InvalidOperationException($"GetInnerArray<{typeof(T).Name}> conversion failed!");
        //            }
        //            return res;
        //        }
        //
        //        private static List<TResult> ConvertListOfMatchingStructs<TSource, TResult>(this List<TSource> source, Func<TSource[], TResult[]> convert)
        //        {
        //            var target = new List<TResult>();
        //            var targetItemsSet = ListInternals<TResult>.ItemsSet;
        //            var sourceItemsGet = ListInternals<TSource>.ItemsGet;
        //            var targetSizeSet = ListInternals<TResult>.SizeSet;
        //
        //            var sourceItemsValue = sourceItemsGet(source);
        //            var targetItems = convert(sourceItemsValue);
        //
        //            targetItemsSet(target, targetItems);
        //            // targetItemsSet.SetValue(target, targetItems, BindingFlags.SetField, new ArrayKeepBinder<TSource, TResult>(), null);
        //            targetSizeSet(target, source.Count);
        //            return target;
        //        }
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

        //        private static class ListInternals<T>
        //        {
        //            public static readonly Func<List<T>, T[]> ItemsGet;
        //            public static readonly Action<List<T>, T[]> ItemsSet;
        //            public static readonly Func<List<T>, int> SizeGet;
        //            public static readonly Action<List<T>, int> SizeSet;
        //
        //            static ListInternals()
        //            {
        //                var itemsField = typeof(List<T>).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic);
        //                ItemsGet = CreateGetFieldDelegate<List<T>, T[]>(itemsField);
        //                ItemsSet = CreateSetFieldDelegate<List<T>, T[]>(itemsField);
        //
        //                var sizeField = typeof(List<T>).GetField("_size", BindingFlags.Instance | BindingFlags.NonPublic);
        //                SizeGet = CreateGetFieldDelegate<List<T>, int>(sizeField);
        //                SizeSet = CreateSetFieldDelegate<List<T>, int>(sizeField);
        //            }
        //
        //            private static Func<TOwner, TValue> CreateGetFieldDelegate<TOwner, TValue>(FieldInfo fieldInfo)
        //            {
        //                ParameterExpression ownerParameter = Expression.Parameter(typeof(TOwner));
        //
        //                var fieldExpression = Expression.Field(
        //                    Expression.Convert(ownerParameter, typeof(TOwner)), fieldInfo);
        //
        //                return Expression.Lambda<Func<TOwner, TValue>>(
        //                    Expression.Convert(fieldExpression, typeof(TValue)),
        //                    ownerParameter).Compile();
        //            }
        //
        //            private static Action<TOwner, TValue> CreateSetFieldDelegate<TOwner, TValue>(FieldInfo fieldInfo)
        //            {
        //                ParameterExpression ownerParameter = Expression.Parameter(typeof(TOwner));
        //                ParameterExpression fieldParameter = Expression.Parameter(typeof(TValue));
        //
        //                var fieldExpression = Expression.Field(
        //                    Expression.Convert(ownerParameter, typeof(TOwner)), fieldInfo);
        //
        //                return Expression.Lambda<Action<TOwner, TValue>>(
        //                    Expression.Assign(fieldExpression,
        //                        Expression.Convert(fieldParameter, fieldInfo.FieldType)),
        //                    ownerParameter, fieldParameter).Compile();
        //            }
        //        }
        //
        //        private class ArrayKeepBinder<TSource, TResult> : Binder
        //        {
        //            public override FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, object value, CultureInfo culture)
        //            {
        //                return Type.DefaultBinder.BindToField(bindingAttr, match, value, culture);
        //            }
        //
        //            public override MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] names, out object state)
        //            {
        //                return Type.DefaultBinder.BindToMethod(bindingAttr, match, ref args, modifiers, culture, names, out state);
        //            }
        //
        //            public override object ChangeType(object value, Type type, CultureInfo culture)
        //            {
        //                if (value.GetType() == typeof(TSource[]) && type == typeof(TResult[]))
        //                {
        //                    return value;
        //                }
        //                throw new NotImplementedException();
        //            }
        //
        //            public override void ReorderArgumentArray(ref object[] args, object state)
        //            {
        //                Type.DefaultBinder.ReorderArgumentArray(ref args, state);
        //            }
        //
        //            public override MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] match, Type[] types, ParameterModifier[] modifiers)
        //            {
        //                return Type.DefaultBinder.SelectMethod(bindingAttr, match, types, modifiers);
        //            }
        //
        //            public override PropertyInfo SelectProperty(BindingFlags bindingAttr, PropertyInfo[] match, Type returnType, Type[] indexes, ParameterModifier[] modifiers)
        //            {
        //                return Type.DefaultBinder.SelectProperty(bindingAttr, match, returnType, indexes, modifiers);
        //            }
        //        }
    }
}