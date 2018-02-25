using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Votyra.Core
{
    public class UnitySyncContext : MonoBehaviour
    {
        public static Thread UnityThread { get; private set; }
        public static SynchronizationContext UnitySynchronizationContext { get; private set; }
        public static TaskScheduler UnityTaskScheduler { get; private set; }

        static UnitySyncContext()
        {
            UnityThread = Thread.CurrentThread;
            UnitySynchronizationContext = SynchronizationContext.Current;
            UnityTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }
    }
}