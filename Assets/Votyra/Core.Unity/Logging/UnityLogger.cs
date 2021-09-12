using System;
using UnityEngine;
using Votyra.Core.Logging;
using Object = UnityEngine.Object;

namespace Votyra.Core.Unity.Logging
{
    public class UnityLogger : IThreadSafeLogger
    {
        private readonly string _name;
        private readonly Object _owner;

        public UnityLogger(string name, Object owner)
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