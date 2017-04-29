using System;
using UnityEngine;

namespace Votyra.Unity.Logging
{
    public class UnityLogger : Common.Logging.ILogger
    {
        public string Name { get; private set; }

        public UnityLogger(string name)
        {
            Name = name;
        }

        public void LogMessage(object message)
        {
            Debug.LogFormat("{0} : {1}", Name, message);
        }

        public void LogError(object message)
        {
            Debug.LogErrorFormat("{0} : {1}", Name, message);
        }

        public void LogException(Exception exception)
        {
            Debug.LogException(exception);
        }

        public void LogWarning(object message)
        {
            Debug.LogWarningFormat("{0} : {1}", Name, message);
        }
    }
}