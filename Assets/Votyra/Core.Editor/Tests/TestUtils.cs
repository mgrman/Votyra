using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;

namespace Votyra.Core
{
    public static class TestUtils
    {
        public static void AssertListEquality<T>(IList<T> expectedResult, IList<T> resultItems, IEqualityComparer<T> comparer = null)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }

            for (var i = 0; i < Math.Max(resultItems.Count, expectedResult.Count); i++)
            {
                if (i >= resultItems.Count)
                {
                    throw new AssertionException($"Expected list has more elements. Expected {expectedResult.Count}, was {resultItems.Count}. Expected:\n{string.Join(", ", expectedResult)}\n\nActual:\n{string.Join(", ", resultItems)}");
                }

                if (i >= expectedResult.Count)
                {
                    throw new AssertionException($"Expected list has less elements. Expected {expectedResult.Count}, was {resultItems.Count}. Expected:\n{string.Join(", ", expectedResult)}\n\nActual:\n{string.Join(", ", resultItems)}");
                }

                if (!comparer.Equals(resultItems[i], expectedResult[i]))
                {
                    throw new AssertionException($"Items as position [{i}] do not match. Expected '{expectedResult[i]}' was '{resultItems[i]}. Expected:\n{string.Join(", ", expectedResult)}\n\nActual:\n{string.Join(", ", resultItems)}");
                }
            }
        }

        public static void UnityAsyncTest(this Func<Task> taskFactory)
        {
            Debug.Log("Creating task.");
            var task = taskFactory();

            UnityAsyncTest(task);
        }

        public static void UnityAsyncTest(this Task task)
        {
            var unitySyncContext = SynchronizationContext.Current;
            ValidateSyncContextType(unitySyncContext);

            AwaitTaskUsingExecMethodOfSyncContext(task, unitySyncContext);
            Exception exception = task.Exception;
            if (exception != null)
            {
                if (exception is AggregateException)
                {
                    var ag = exception as AggregateException;
                    if (ag.InnerExceptions.Count == 1)
                    {
                        exception = ag.InnerExceptions[0];
                    }
                }

                ExceptionDispatchInfo.Capture(exception)
                    .Throw();
            }
        }

        private static void AwaitTaskUsingExecMethodOfSyncContext(Task task, SynchronizationContext context)
        {
            var execAction = GetExecSyncContextAction(context);

            while (!task.IsCompleted)
            {
                execAction();
            }

            Debug.Log("Task completed.");
        }

        private static void ValidateSyncContextType(SynchronizationContext unitySyncContext)
        {
            var syncContextName = unitySyncContext?.GetType()
                .FullName ?? "<null>";
            if (syncContextName != "UnityEngine.UnitySynchronizationContext")
            {
                throw new AssertionException($"AsyncTerrainGeneration task cannot be tested with {syncContextName} as SynchronizationContext! UnitySynchronizationContext is required!");
            }
        }

        private static Action GetExecSyncContextAction(SynchronizationContext syncContext)
        {
            var execMethod = syncContext.GetType()
                .GetMethod("Exec", BindingFlags.Instance | BindingFlags.NonPublic);
            if (execMethod == null)
            {
                throw new AssertionException("AsyncTerrainGeneration task cannot be tested without Exec() method on UnitySynchronizationContext!");
            }

            var execAction = (Action)execMethod.CreateDelegate(typeof(Action), syncContext);
            return execAction;
        }
    }
}
