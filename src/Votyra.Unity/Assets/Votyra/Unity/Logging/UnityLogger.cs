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
            Debug.Log(Format(message), Owner as UnityEngine.Object);
        }

        public void LogError(object message)
        {
            Debug.LogError(Format(message), Owner as UnityEngine.Object);
        }

        public void LogException(Exception exception)
        {
            Debug.LogException(exception, Owner as UnityEngine.Object);
        }

        public void LogWarning(object message)
        {
            Debug.LogWarning(Format(message), Owner as UnityEngine.Object);
        }

        private string Format(object message)
        {
            return $"{Name} : {message}";//\r\n{t}";
        }

    }
}