using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;

namespace Votyra.Core
{
    [TestFixture]
    public class UniRxSchedulerTest
    {
        [Test]
        public void Test1()
        {

        }

        [Test]
        public void Test()
        {
            TestUtils.UnityAsyncTest(async () =>
            {
                await TestAsync();

                await Task.Run(() => Thread.Sleep(1000));
            });
        }



        private async Task TestAsync()
        {
            var cts = new TaskCompletionSource<bool>();

            var _ = Task.Run(async () =>
              {
                  await Task.Delay(5000);
                  cts.SetResult(true);
              });

            await cts.Task;
        }

    }
}