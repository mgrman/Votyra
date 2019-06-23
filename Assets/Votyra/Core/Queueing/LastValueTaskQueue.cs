using System;
using System.Threading.Tasks;
using Votyra.Core.Logging;
using Votyra.Core.Utils;

namespace Votyra.Core.Queueing
{
    public class LastValueTaskQueue<T> : IWorkQueue<T> where T : IDisposable
    {
        private readonly string _name;
        private readonly Action<T> _updateFunction;
        private bool _stopped;
        private object _taskLock = new object();
        private bool _activeTask;
        private (bool HasValue, T Value) _queuedUpdate = (false, default);

        private void DisposeAndSet(bool hasValue, T value)
        {
            lock (_taskLock)
            {
                if (_queuedUpdate.HasValue)
                {
                    _queuedUpdate.Value?.Dispose();
                }

                _queuedUpdate = (hasValue, value);
            }
        }

        private (bool, T) GetQueued()
        {
            (bool, T) activeContext;
            lock (_taskLock)
            {
                activeContext = _queuedUpdate;
                _queuedUpdate = (false, default);
            }

            return activeContext;
        }

        public LastValueTaskQueue(string name, Action<T> updateFunction)
        {
            _name = name;
            _updateFunction = updateFunction;
        }

        public void QueueNew(T context)
        {
            bool startNewTask;
            lock (_taskLock)
            {
                DisposeAndSet(true, context);
                startNewTask = !_activeTask;
                _activeTask = true;
            }

            if (startNewTask)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        int counter = 0;
                        while (true)
                        {
                            (bool HasValue, T Value) activeContext;
                            lock (_taskLock)
                            {
                                activeContext = GetQueued();
                                _activeTask = activeContext.HasValue;
                                if (!_activeTask)
                                {
                                    return;
                                }
                            }

                            try
                            {
                                _updateFunction(activeContext.Value);
                            }
                            catch (Exception ex)
                            {
                                StaticLogger.LogError($"Error in {this.GetType().GetDisplayName()} named {_name}:");
                                StaticLogger.LogException(ex);
                            }
                            finally
                            {
                                try
                                {
                                    activeContext.Value.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    StaticLogger.LogError($"Error disposing context {activeContext.GetHashCode()} in {this.GetType().GetDisplayName()} named {_name}:");
                                    StaticLogger.LogException(ex);
                                }
                            }

                            counter++;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticLogger.LogException(ex);
                    }
                });
            }
        }

        public void Stop()
        {
            DisposeAndSet(false, default);
        }
    }
}