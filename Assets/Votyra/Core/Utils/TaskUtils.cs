using System;
using System.Threading.Tasks;

namespace Votyra.Core.Utils
{
    public static class TaskUtils
    {
        public static Task RunOrNot(Action action, bool async)
        {
            if (async)
                return Task.Run(action);

            action();
            return Task.CompletedTask;
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