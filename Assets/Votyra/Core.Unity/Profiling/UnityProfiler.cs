using System.Threading;

using UnityEngine.Profiling;

namespace Votyra.Core.Profiling
{
    public struct UnityProfiler : IProfiler
    {
        private readonly bool _calledProfiler;

        public UnityProfiler(string name, object owner)
        {
            if (Thread.CurrentThread == UnitySyncContext.UnityThread)
            {
                if (owner is UnityEngine.Object)
                    Profiler.BeginSample(name, owner as UnityEngine.Object);
                else
                    Profiler.BeginSample(name);
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