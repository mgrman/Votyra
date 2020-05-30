using System;
using System.Threading.Tasks;
using Votyra.Core.Logging;
using Votyra.Core.Utils;

namespace Votyra.Core.Queueing
{
    public class LastValueTaskQueue<T> : IWorkQueue<T> where T : IDisposable
    {
        private readonly TaskFactory taskFactory;
        private readonly object taskLock = new object();
        private bool activeTask;
        private (bool HasValue, T Value) queuedUpdate = (false, default);

        public LastValueTaskQueue()
        {
            this.taskFactory = new TaskFactory();
        }

        public event Action<T> DoWork;

        public void QueueNew(T context)
        {
            bool startNewTask;
            lock (this.taskLock)
            {
                this.DisposeAndSet(true, context);
                startNewTask = !this.activeTask;
                this.activeTask = true;
            }

            if (startNewTask)
            {
                this.taskFactory.StartNew(this.TaskUpdate);
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
                    lock (this.taskLock)
                    {
                        activeContext = this.GetQueued();
                        this.activeTask = activeContext.HasValue;
                        if (!this.activeTask)
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
            lock (this.taskLock)
            {
                if (this.queuedUpdate.HasValue)
                {
                    this.queuedUpdate.Value?.Dispose();
                }

                this.queuedUpdate = (hasValue, value);
            }
        }

        private (bool, T) GetQueued()
        {
            (bool, T) activeContext;
            lock (this.taskLock)
            {
                activeContext = this.queuedUpdate;
                this.queuedUpdate = (false, default);
            }

            return activeContext;
        }
    }
}