using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Votyra.Core
{
    [TestFixture,]
    public class TestUtilsTest
    {
        private async Task TestAsync()
        {
            var cts = new TaskCompletionSource<bool>();

            var _ = Task.Run(async () =>
            {
                await Task.Delay(500);
                cts.SetResult(true);
            });

            await cts.Task;
        }

        [Test,]
        public void UnityAsyncTest_RunTheAsyncFunctionGiven_PropagatesThrownExceptions()
        {
            Assert.Throws<AssertionException>(() =>
            {
                TestUtils.UnityAsyncTest(async () =>
                {
                    await this.TestAsync();

                    await Task.Run(() => Thread.Sleep(100));
                    Assert.IsTrue(false);
                });
            });
        }

        [Test,]
        public void UnityAsyncTest_RunTheAsyncFunctionGiven_WaitsForItToFinish()
        {
            var finished = false;
            TestUtils.UnityAsyncTest(async () =>
            {
                await this.TestAsync();

                await Task.Run(() => Thread.Sleep(100));
                finished = true;
            });

            Assert.IsTrue(finished);
        }
    }
}
