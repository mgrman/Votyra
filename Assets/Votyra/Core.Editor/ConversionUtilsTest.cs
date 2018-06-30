using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.Editor
{
    [TestFixture]
    public class ConversionUtilsTest
    {
        [Test]
        [TestCase(10)]
        [TestCase(13)]
        [TestCase(103)]
        [TestCase(1000)]
        [TestCase(10000)]
        public void ArrayConversionTest(int count)
        {
            int tests = 10000;
            Func<Vector3f[]> createNewArray = () =>
            {
                var list = Enumerable.Range(0, count).Select(o => new Vector3f(o, o, o)).ToArray();
                return list;
            };

            DateTime start = DateTime.UtcNow;
            for (int test = 0; test < tests; test++)
            {
                createNewArray();
            }
            TimeSpan nothing = DateTime.UtcNow - start;

            start = DateTime.UtcNow;
            for (int test = 0; test < tests; test++)
            {
                var initialList = createNewArray();

                var newList = new UnityEngine.Vector3[initialList.Length];
                for (int i = 0; i < initialList.Length; i++)
                {
                    var vec = initialList[i];
                    newList[i] = new UnityEngine.Vector3(vec.X, vec.Y, vec.Z);
                }
            }
            TimeSpan forLoop = DateTime.UtcNow - start - nothing;

            start = DateTime.UtcNow;
            for (int test = 0; test < tests; test++)
            {
                var initialList = createNewArray();

                initialList.ToVector3();
            }
            TimeSpan structConversion = DateTime.UtcNow - start - nothing;

            Debug.Log("forLoop: " + (int)forLoop.TotalMilliseconds);
            Debug.Log("newListWithSameArray: " + (int)structConversion.TotalMilliseconds);
        }

        [Test]
        [TestCase(10)]
        [TestCase(13)]
        [TestCase(103)]
        [TestCase(1000)]
        [TestCase(10000)]
        public void ListConversionTest(int count)
        {
            int tests = 10000;
            Func<List<Vector3f>> createNewList = () =>
            {
                var list = Enumerable.Range(0, count).Select(o => new Vector3f(o, o, o)).ToList();
                list.Capacity = list.Capacity + 7;
                return list;
            };
            // Debug.Log("capacity: " + createNewList().Capacity);

            DateTime start = DateTime.UtcNow;
            for (int test = 0; test < tests; test++)
            {
                createNewList();
            }
            TimeSpan nothing = DateTime.UtcNow - start;

            start = DateTime.UtcNow;
            for (int test = 0; test < tests; test++)
            {
                var initialList = createNewList();

                var newList = new List<UnityEngine.Vector3>(initialList.Count);
                for (int i = 0; i < initialList.Count; i++)
                {
                    var vec = initialList[i];
                    newList.Add(new UnityEngine.Vector3(vec.X, vec.Y, vec.Z));
                }
            }
            TimeSpan forLoop = DateTime.UtcNow - start - nothing;

            start = DateTime.UtcNow;
            for (int test = 0; test < tests; test++)
            {
                var initialList = createNewList();

                initialList.ToVector3Array();
            }
            TimeSpan innerArray = DateTime.UtcNow - start - nothing;

            start = DateTime.UtcNow;
            for (int test = 0; test < tests; test++)
            {
                var initialList = createNewList();

                initialList.ToVector3List();
            }
            TimeSpan newListWithSameArray = DateTime.UtcNow - start - nothing;

            Debug.Log("forLoop: " + (int)forLoop.TotalMilliseconds);
            Debug.Log("innerArray: " + (int)innerArray.TotalMilliseconds);
            Debug.Log("newListWithSameArray: " + (int)newListWithSameArray.TotalMilliseconds);
        }
    }
}