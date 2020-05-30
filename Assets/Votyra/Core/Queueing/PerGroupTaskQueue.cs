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
        private readonly Dictionary<Vector2i, GroupUpdateData> queuedUpdates = new Dictionary<Vector2i, GroupUpdateData>();
        private readonly TaskFactory taskFactory;
        private readonly object taskLock = new object();
        private readonly HashSet<Vector2i> updateTasks = new HashSet<Vector2i>();

        public PerGroupTaskQueue()
        {
            this.taskFactory = new TaskFactory();
        }

        public event Action<GroupUpdateData> DoWork;

        public void QueueNew(GroupUpdateData context)
        {
            bool startNewTask;
            bool disposeOldValue;
            GroupUpdateData oldValue;
            lock (this.taskLock)
            {
                disposeOldValue = this.queuedUpdates.TryGetValue(context.Group, out oldValue);
                if (disposeOldValue && oldValue.ForceUpdate)
                {
                    context = new GroupUpdateData(context.Group, context.Context, context.Mesh, true);
                }

                this.queuedUpdates[context.Group] = context;

                startNewTask = !this.updateTasks.Contains(context.Group);
                this.updateTasks.Add(context.Group);
            }

            if (disposeOldValue)
            {
                oldValue.Dispose();
            }

            if (startNewTask)
            {
                this.taskFactory.StartNew(this.TaskUpdate, context.Group);
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
                    lock (this.taskLock)
                    {
                        if (this.queuedUpdates.TryGetValue(group, out activeContext))
                        {
                            this.queuedUpdates.Remove(group);
                        }
                        else
                        {
                            this.updateTasks.Remove(group);
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
