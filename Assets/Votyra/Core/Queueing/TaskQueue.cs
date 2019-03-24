using System;
using System.Threading.Tasks;
using UnityEngine;
using Votyra.Core.Pooling;
using Votyra.Core.Utils;

namespace Votyra.Core.Queueing
{
    public class TaskQueue<T> where T : class, IDisposable
    {
        private readonly string _name;
        private readonly Func<T, Task> _updateAsyncFunction;
        private readonly Action<T> _updateFunction;
        private bool _stopped;
        private object _taskLock = new object();
        private bool _activeTask;
        private T _queuedUpdate = null;

        private void DisposeAndSet(T value)
        {
            lock (_taskLock)
            {
                if (_queuedUpdate != null)
                {
                    _queuedUpdate.Dispose();
                }

                _queuedUpdate = value;
            }
        }

        private T GetQueued()
        {
            T activeContext;
            lock (_taskLock)
            {
                activeContext = _queuedUpdate;
                _queuedUpdate = null;
            }

            return activeContext;
        }

        public TaskQueue(string name, Func<T, Task> updateFunction)
        {
            _name = name;
            _updateAsyncFunction = updateFunction;
        }

        public TaskQueue(string name, Action<T> updateFunction)
        {
            _name = name;
            _updateFunction = updateFunction;
        }

        public void QueueNew(T context)
        {
            bool startNewTask;
            lock (_taskLock)
            {
                DisposeAndSet(context);
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
                            T activeContext;
                            lock (_taskLock)
                            {
                                activeContext = GetQueued();
                                _activeTask = activeContext != null;
                                if (!_activeTask)
                                {
                                    return;
                                }
                            }

                            try
                            {
                                if (_updateAsyncFunction != null)
                                {
                                    await _updateAsyncFunction(activeContext);
                                }
                                
                                if (_updateFunction != null)
                                {
                                    _updateFunction(activeContext);
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError($"Error in {this.GetType().GetDisplayName()} named {_name}:");
                                Debug.LogException(ex);
                            }
                            finally
                            {
                                try
                                {
                                    activeContext.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogError($"Error disposing context {activeContext.GetHashCode()} in {this.GetType().GetDisplayName()} named {_name}:");
                                    Debug.LogException(ex);
                                }
                            }

                            counter++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                });
            }
        }

        public void Stop()
        {
            DisposeAndSet(null);
        }
    }
}