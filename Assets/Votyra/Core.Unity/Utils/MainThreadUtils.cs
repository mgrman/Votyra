using System;
using System.Threading.Tasks;
using UniRx;
using UniRx.Async;

namespace Votyra.Core.Unity.Utils
{
    public static class MainThreadUtils
    {
        public static void RunOnMainThread(Action action)
        {
            if (MainThreadDispatcher.IsInMainThread)
                action();
            else
                RunOnMainThreadAsync(action)
                    .Wait();
        }

        public static Task RunOnMainThreadAsync(Action action)
        {
            return Task.Run(async () =>
            {
                await UniTask.SwitchToMainThread();
                action();
            });
        }
    }
}