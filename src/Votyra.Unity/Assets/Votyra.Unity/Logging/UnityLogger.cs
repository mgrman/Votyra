using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Votyra.Logging;

namespace Votyra.Unity.Logging
{
    public class UnityLogger : IThreadSafeLogger
    {
        public string Name { get; set; }
        public object Owner { get; set; }

        public UnityLogger(string name, object owner)
        {
            Name = name;
            Owner = owner;
        }

        public void LogMessage(object message)
        {
            InvokeOnUnityThreadIfRequired(() => Debug.Log(Format(message), Owner as UnityEngine.Object));
        }

        public void LogError(object message)
        {
            InvokeOnUnityThreadIfRequired(() => Debug.LogError(Format(message), Owner as UnityEngine.Object));
        }

        public void LogException(Exception exception)
        {
            InvokeOnUnityThreadIfRequired(() => Debug.LogException(exception, Owner as UnityEngine.Object));
        }

        public void LogWarning(object message)
        {
            InvokeOnUnityThreadIfRequired(() => Debug.LogWarning(Format(message), Owner as UnityEngine.Object));
        }

        private string Format(object message)
        {
            System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
            return $"{Name} : {message}";//\r\n{t}";
        }

        private void InvokeOnUnityThreadIfRequired(Action action)
        {
            if (Thread.CurrentThread == UnitySyncContext.UnityThread)
            {
                action();
            }
            else
            {
                Task.Factory.StartNew(
                    action,
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    UnitySyncContext.UnityTaskScheduler);
            }
        }
    }
}