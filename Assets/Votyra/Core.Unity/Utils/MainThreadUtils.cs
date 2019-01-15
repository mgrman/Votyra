using System;
using System.Collections;
using System.Threading.Tasks;
using UniRx;
using UniRx.Async;

namespace Votyra.Core.Utils
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
                })
                .ConfiguraAwaitFluent(false);
        }

        public static UniTask RunOnMainThreadUniAsync(Action action)
        {
            return UniTask.Run(async () =>
            {
                await UniTask.SwitchToMainThread();
                action();
            }, false);
        }

        public static Task RunOnMainThreadAsync(Func<Task> action)
        {
            return Task.Run(async () =>
                {
                    await UniTask.SwitchToMainThread();
                    await action();
                })
                .ConfiguraAwaitFluent(false);
        }

        public static Task<T> RunOnMainThreadAsync<T>(Func<T> action)
        {
            return Task.Run(async () =>
                {
                    await UniTask.SwitchToMainThread();
                    return action();
                })
                .ConfiguraAwaitFluent(false);
        }

        public static Task<T> RunOnMainThreadAsync<T>(Func<Task<T>> action)
        {
            return Task.Run(async () =>
                {
                    await UniTask.SwitchToMainThread();
                    return await action();
                })
                .ConfiguraAwaitFluent(false);
        }

    }
}