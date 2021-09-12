using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Votyra.Core.Unity
{
    public class UnitySyncContext : MonoBehaviour
    {
        public static Thread UnityThread { get; private set; }
        
        public static SynchronizationContext UnitySynchronizationContext { get; private set; }
        
        public static TaskScheduler UnityTaskScheduler { get; private set; }

        protected virtual void Awake()
        {
            UnityThread = System.Threading.Thread.CurrentThread;
            UnitySynchronizationContext = SynchronizationContext.Current;
            UnityTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }
    }
}