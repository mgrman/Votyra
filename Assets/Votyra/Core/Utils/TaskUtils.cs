using System;
using System.Threading.Tasks;
using UniRx.Async;

namespace Votyra.Core.Utils
{
    public static class TaskUtils
    {

        public static Task RunOnMainThread(Action action)
        {
            return Task.Run(async () =>
                {
                    await UniTask.SwitchToMainThread();
                    action();
                })
                .ConfiguraAwaitFluent(false);
        }

        public static Task RunOnMainThread(Func<Task> action)
        {
            return Task.Run(async () =>
                {
                    await UniTask.SwitchToMainThread();
                    await action();
                })
                .ConfiguraAwaitFluent(false);
        }

        public static  Task<T> RunOnMainThread<T>(Func<T> action)
        {
           return Task.Run(async () =>
                {
                    await UniTask.SwitchToMainThread();
                    return action();
                })
                .ConfiguraAwaitFluent(false);
        }

        public static Task<T> RunOnMainThread<T>(Func<Task<T>> action)
        {
            return Task.Run(async () =>
                {
                    await UniTask.SwitchToMainThread();
                   return await action();
                })
                .ConfiguraAwaitFluent(false);
        }

        public static Task ConfiguraAwaitFluent(this Task task, bool continueOnCapturedContext)
        {
            task.ConfigureAwait(continueOnCapturedContext);
            return task;
        }

        public static Task<T> ConfiguraAwaitFluent<T>(this Task<T> task, bool continueOnCapturedContext)
        {
            task.ConfigureAwait(continueOnCapturedContext);
            return task;
        }
    }
}