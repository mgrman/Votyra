using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using NUnit.Framework;

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
        public void ListConversionTest(int count)
        {
            int tests = 10000;
            Func<List<Vector3f>> createNewList = () =>
            {
                var list = Enumerable.Range(0, count).Select(o => new Vector3f(o, o, o)).ToList();
                list.Capacity = list.Capacity + 7;
                return list;
            };
            Debug.Log("capacity: " + createNewList().Capacity);
            TimeSpan forLoop = new TimeSpan();
            for (int test = 0; test < tests; test++)
            {
                var initialList = createNewList();
                DateTime start = DateTime.UtcNow;

                var newList = new List<UnityEngine.Vector3>(initialList.Count);
                for (int i = 0; i < initialList.Count; i++)
                {
                    var vec = initialList[i];
                    newList.Add(new UnityEngine.Vector3(vec.x, vec.y, vec.z));
                }

                forLoop += DateTime.UtcNow - start;
            }

            TimeSpan innerArray = new TimeSpan();
            for (int test = 0; test < tests; test++)
            {
                var initialList = createNewList();
                DateTime start = DateTime.UtcNow;

                var newArray = initialList.ToVector3Array();

                innerArray += DateTime.UtcNow - start;
            }

            TimeSpan newListWithSameArray = new TimeSpan();
            for (int test = 0; test < tests; test++)
            {
                var initialList = createNewList();
                DateTime start = DateTime.UtcNow;

                var newArray = initialList.ToVector3List();

                newListWithSameArray += DateTime.UtcNow - start;
            }

            Debug.Log("forLoop: " + (int) forLoop.TotalMilliseconds);
            Debug.Log("innerArray: " + (int) innerArray.TotalMilliseconds);
            Debug.Log("newListWithSameArray: " + (int) newListWithSameArray.TotalMilliseconds);
        }

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

            TimeSpan forLoop = new TimeSpan();
            for (int test = 0; test < tests; test++)
            {
                var initialList = createNewArray();
                DateTime start = DateTime.UtcNow;

                var newList = new UnityEngine.Vector3[initialList.Length];
                for (int i = 0; i < initialList.Length; i++)
                {
                    var vec = initialList[i];
                    newList[i] = new UnityEngine.Vector3(vec.x, vec.y, vec.z);
                }

                forLoop += DateTime.UtcNow - start;
            }

            TimeSpan structConversion = new TimeSpan();
            for (int test = 0; test < tests; test++)
            {
                var initialList = createNewArray();
                DateTime start = DateTime.UtcNow;

                var newArray = initialList.ToVector3();

                structConversion += DateTime.UtcNow - start;
            }

            Debug.Log("forLoop: " + (int) forLoop.TotalMilliseconds);
            Debug.Log("newListWithSameArray: " + (int) structConversion.TotalMilliseconds);
        }
    }
}