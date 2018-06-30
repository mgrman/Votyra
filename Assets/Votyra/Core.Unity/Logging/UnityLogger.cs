using System;
using UnityEngine;

namespace Votyra.Core.Logging
{
    public class UnityLogger : IThreadSafeLogger
    {
        private readonly string _name;
        private readonly UnityEngine.Object _owner;

        public UnityLogger(string name, UnityEngine.Object owner)
        {
            _name = name;
            _owner = owner;
        }

        public void LogError(object message)
        {
            Debug.LogError(Format(message), _owner);
        }

        public void LogException(Exception exception)
        {
            Debug.LogException(exception, _owner);
        }

        public void LogMessage(object message)
        {
            Debug.Log(Format(message), _owner);
        }

        public void LogWarning(object message)
        {
            Debug.LogWarning(Format(message), _owner);
        }

        private string Format(object message)
        {
            return $"{message}\n{_name}";
        }
    }
}