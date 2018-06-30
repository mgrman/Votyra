using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Votyra.Core
{
    public class UnitySyncContext : MonoBehaviour
    {
        public static SynchronizationContext UnitySynchronizationContext { get; private set; }
        public static TaskScheduler UnityTaskScheduler { get; private set; }
        public static Thread UnityThread { get; private set; }

        protected virtual void Awake()
        {
            UnityThread = Thread.CurrentThread;
            UnitySynchronizationContext = SynchronizationContext.Current;
            UnityTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }
    }
}