using System.Threading;

using UnityEngine.Profiling;
using Votyra.Core.Profiling;

namespace Votyra.Core.Profiling
{
    public struct UnityProfiler : IProfiler
    {
        private readonly bool _calledProfiler;

        public UnityProfiler(string name, object owner)
        {
            if (Thread.CurrentThread == UnitySyncContext.UnityThread)
            {
                Profiler.BeginSample(name, owner as UnityEngine.Object);
                _calledProfiler = true;
            }
            else
            {
                _calledProfiler = false;
            }
        }

        public void Dispose()
        {
            if (_calledProfiler)
                Profiler.EndSample();
        }
    }
}
