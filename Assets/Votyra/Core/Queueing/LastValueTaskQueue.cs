using System;
using System.Threading.Tasks;
using Votyra.Core.Logging;
using Votyra.Core.Utils;

namespace Votyra.Core.Queueing
{
    public class LastValueTaskQueue<T> : IWorkQueue<T> where T : IDisposable
    {
        private readonly TaskFactory _taskFactory;
        private readonly object _taskLock = new object();
        private bool _activeTask;
        private (bool HasValue, T Value) _queuedUpdate = (false, default);
        private bool _stopped;

        public LastValueTaskQueue()
        {
            this._taskFactory = new TaskFactory();
        }

        public event Action<T> DoWork;

        public void QueueNew(T context)
        {
            bool startNewTask;
            lock (this._taskLock)
            {
                this.DisposeAndSet(true, context);
                startNewTask = !this._activeTask;
                this._activeTask = true;
            }

            if (startNewTask)
            {
                this._taskFactory.StartNew(this.TaskUpdate);
            }
        }

        private void TaskUpdate()
        {
            try
            {
                var counter = 0;
                while (true)
                {
                    (bool HasValue, T Value) activeContext;
                    lock (this._taskLock)
                    {
                        activeContext = this.GetQueued();
                        this._activeTask = activeContext.HasValue;
                        if (!this._activeTask)
                        {
                            return;
                        }
                    }

                    try
                    {
                        this.DoWork?.Invoke(activeContext.Value);
                    }
                    catch (Exception ex)
                    {
                        StaticLogger.LogError($"Error in {this.GetType().GetDisplayName()}:");
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
                            StaticLogger.LogError($"Error disposing context {activeContext.GetHashCode()} in {this.GetType().GetDisplayName()}:");
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
        }

        private void DisposeAndSet(bool hasValue, T value)
        {
            lock (this._taskLock)
            {
                if (this._queuedUpdate.HasValue)
                {
                    this._queuedUpdate.Value?.Dispose();
                }

                this._queuedUpdate = (hasValue, value);
            }
        }

        private (bool, T) GetQueued()
        {
            (bool, T) activeContext;
            lock (this._taskLock)
            {
                activeContext = this._queuedUpdate;
                this._queuedUpdate = (false, default);
            }

            return activeContext;
        }
    }
}
