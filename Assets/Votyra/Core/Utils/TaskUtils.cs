using System;
using System.Collections;
using System.Threading.Tasks;
using UniRx;
using UniRx.Async;
using UnityEngine;

namespace Votyra.Core.Utils
{
    public static class TaskUtils
    {
        public static void RunOnMainThread(Action action)
        {
            if (MainThreadDispatcher.IsInMainThread)
                action();
            else
                RunOnMainThreadAsync(action)
                    .Wait();
        }

        public static Task RunOrNot(Action action, bool async)
        {
            if (async)
            {
                return Task.Run(action);
            }

            action();
            return Task.CompletedTask;
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

        public static void StartCoroutine(IEnumerator coroutine)
        {
            CoroutineRunner.Instance.StartCoroutine(coroutine);
        }

        private class CoroutineRunner : MonoBehaviour
        {
            private static CoroutineRunner _instance;

            public static CoroutineRunner Instance
            {
                get
                {
                    if (_instance == null)
                        _instance = new GameObject(nameof(CoroutineRunner)).AddComponent<CoroutineRunner>();

                    return _instance;
                }
            }
        }
    }
}