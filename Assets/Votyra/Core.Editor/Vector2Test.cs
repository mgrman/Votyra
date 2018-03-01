using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Votyra.Core.Models;

namespace Votyra.Core.Editor
{
    [TestFixture]
    public class Vector2Test
    {
        [Test]
        public void Test()
        {
            float x = 5;
            float y = 7.2f;
            var value = new Vector2_compatible(x, y);
            Assert.AreEqual(x, value.Unity.x);
            Assert.AreEqual(y, value.Unity.y);
        }
    }
}