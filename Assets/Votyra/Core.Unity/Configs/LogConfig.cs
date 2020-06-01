namespace Votyra.Core.Images
{
    public class LogConfig : ILogConfig
    {
        public LogConfig([ConfigInject("logLevel")]LogLevel logLevel)
        {
            this.LogLevel = logLevel;
        }

        public LogLevel LogLevel { get; }
    }
}
