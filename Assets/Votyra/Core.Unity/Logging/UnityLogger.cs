using System;
using UnityEngine;
using UnityEngineObject = UnityEngine.Object;

namespace Votyra.Core.Logging
{
    //Should be moved to a separete dll, since then Unity skips it when double clicking log message and goes to caller
    public class UnityLogger : IThreadSafeLogger
    {
        private readonly string _name;
        private readonly UnityEngineObject _owner;

        public UnityLogger(string name, UnityEngineObject owner)
        {
            _name = name;
            _owner = owner;
        }

        public void LogMessage(object message)
        {
            Debug.Log(Format(message), _owner);
        }

        public void LogError(object message)
        {
            Debug.LogError(Format(message), _owner);
        }

        public void LogException(Exception exception)
        {
            Debug.LogException(exception, _owner);
        }

        public void LogWarning(object message)
        {
            Debug.LogWarning(Format(message), _owner);
        }

        private string Format(object message) => $"{message}\n{_name}";
    }
}