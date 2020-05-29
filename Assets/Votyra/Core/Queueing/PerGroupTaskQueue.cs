using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Votyra.Core.Logging;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.Queueing
{
    public class PerGroupTaskQueue : IWorkQueue<GroupUpdateData>
    {
        private readonly Dictionary<Vector2i, GroupUpdateData> _queuedUpdates = new Dictionary<Vector2i, GroupUpdateData>();
        private readonly TaskFactory _taskFactory;
        private readonly object _taskLock = new object();
        private readonly HashSet<Vector2i> _updateTasks = new HashSet<Vector2i>();

        public PerGroupTaskQueue()
        {
            this._taskFactory = new TaskFactory();
        }

        public event Action<GroupUpdateData> DoWork;

        public void QueueNew(GroupUpdateData context)
        {
            bool startNewTask;
            bool disposeOldValue;
            GroupUpdateData oldValue;
            lock (this._taskLock)
            {
                disposeOldValue = this._queuedUpdates.TryGetValue(context.Group, out oldValue);
                if (disposeOldValue && oldValue.ForceUpdate)
                {
                    context = new GroupUpdateData(context.Group, context.Context, context.Mesh, true);
                }

                this._queuedUpdates[context.Group] = context;

                startNewTask = !this._updateTasks.Contains(context.Group);
                this._updateTasks.Add(context.Group);
            }

            if (disposeOldValue)
            {
                oldValue.Dispose();
            }

            if (startNewTask)
            {
                this._taskFactory.StartNew(this.TaskUpdate, context.Group);
            }
        }

        private void TaskUpdate(object arg)
        {
            var context = (Vector2i)arg;
            this.TaskUpdate(context);
        }

        private void TaskUpdate(Vector2i group)
        {
            try
            {
                var counter = 0;
                while (true)
                {
                    GroupUpdateData activeContext;
                    lock (this._taskLock)
                    {
                        if (this._queuedUpdates.TryGetValue(group, out activeContext))
                        {
                            this._queuedUpdates.Remove(group);
                        }
                        else
                        {
                            this._updateTasks.Remove(group);
                            return;
                        }
                    }

                    try
                    {
                        this.DoWork?.Invoke(activeContext);
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
                            activeContext.Dispose();
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
    }
}
