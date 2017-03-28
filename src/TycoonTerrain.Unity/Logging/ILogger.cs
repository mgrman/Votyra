using System;
using TycoonTerrain.Common.Logging;
using UnityEngine;

namespace TycoonTerrain.Unity.Logging
{
    public class UnityLogger: Common.Logging.ILogger
    {
        public string Name { get; }

        public UnityLogger(string name)
        {
            Name = name;
        }

       public void LogMessage(object message)
        {
            Debug.Log($"{Name} : {message}");
        }

        public void LogError(object message)
        {
            Debug.LogError($"{Name} : {message}");
        }

        public void LogException(Exception exception)
        {
            Debug.LogException(exception);
        }

        public void LogWarning(object message)
        {
            Debug.LogWarning($"{Name} : {message}");
        }
    }
}