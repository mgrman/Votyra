using System;
using UnityEngine;

namespace Votyra.Core.Logging
{
    // Should be moved to a separete dll, since then Unity skips it when double clicking log message and goes to caller
    public class UnityLogger : IThreadSafeLogger
    {
        private readonly string name;
        private readonly UnityEngine.Object owner;
        private readonly Func<LogLevel> getLogLevel;

        public UnityLogger(string name, UnityEngine.Object owner, ILogConfig logConfig)
            : this(name, owner, () => logConfig.LogLevel)
        {
        }

        public UnityLogger(string name, UnityEngine.Object owner, Func<LogLevel> getLogLevel)
        {
            this.name = name;
            this.owner = owner;
            this.getLogLevel = getLogLevel;
        }

        public void LogDebug(object message)
        {
            this.LogMessage(LogLevel.Debug, message);
        }

        public void LogInfo(object message)
        {
            this.LogMessage(LogLevel.Info, message);
        }

        public void LogError(object message)
        {
            this.LogMessage(LogLevel.Error, message);
        }

        public void LogWarning(object message)
        {
            this.LogMessage(LogLevel.Warnings, message);
        }

        private void LogMessage(LogLevel messageLogLevel, object message)
        {
            var logLevelLimit = this.getLogLevel();
            if (logLevelLimit < messageLogLevel)
            {
                return;
            }

            switch (messageLogLevel)
            {
                case LogLevel.None:
                    break;
                case LogLevel.Error:
                    Debug.LogError(this.Format(messageLogLevel, message), this.owner);
                    break;
                case LogLevel.Warnings:
                    Debug.LogWarning(this.Format(messageLogLevel, message), this.owner);
                    break;
                case LogLevel.Info:
                case LogLevel.Debug:
                default:
                    Debug.Log(this.Format(messageLogLevel, message), this.owner);
                    break;
            }
        }

        private string Format(LogLevel logLevel, object message) => $"[{logLevel}] {message}\n{this.name}";
    }
}
