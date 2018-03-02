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
    public static class TestUtils
    {
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
            var syncContextName = unitySyncContext?.GetType().FullName ?? "<null>";
            if (syncContextName != "UnityEngine.UnitySynchronizationContext")
            {
                throw new AssertionException($"Async task cannot be tested with {syncContextName} as SynchronizationContext! UnitySynchronizationContext is required!");
            }
        }

        private static Action GetExecSyncContextAction(SynchronizationContext syncContext)
        {
            var execMethod = syncContext.GetType().GetMethod("Exec", BindingFlags.Instance | BindingFlags.NonPublic);
            if (execMethod == null)
            {
                throw new AssertionException("Async task cannot be tested without Exec() method on UnitySynchronizationContext!");
            }
            var execAction = (Action)execMethod.CreateDelegate(typeof(Action), syncContext);
            return execAction;
        }
    }
}