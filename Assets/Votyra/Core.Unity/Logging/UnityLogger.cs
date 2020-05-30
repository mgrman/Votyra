using System;
using UnityEngine;
using UnityEngineObject = UnityEngine.Object;

namespace Votyra.Core.Logging
{
    // Should be moved to a separete dll, since then Unity skips it when double clicking log message and goes to caller
    public class UnityLogger : IThreadSafeLogger
    {
        private readonly string name;
        private readonly UnityEngineObject owner;

        public UnityLogger(string name, UnityEngineObject owner)
        {
            this.name = name;
            this.owner = owner;
        }

        public void LogMessage(object message)
        {
            Debug.Log(this.Format(message), this.owner);
        }

        public void LogError(object message)
        {
            Debug.LogError(this.Format(message), this.owner);
        }

        public void LogException(Exception exception)
        {
            Debug.LogException(exception, this.owner);
        }

        public void LogWarning(object message)
        {
            Debug.LogWarning(this.Format(message), this.owner);
        }

        private string Format(object message) => $"{message}\n{this.name}";
    }
}
