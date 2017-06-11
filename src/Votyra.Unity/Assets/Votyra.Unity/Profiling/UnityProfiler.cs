using System.Threading;
using Votyra.Profiling;
using UnityEngine.Profiling;

namespace Votyra.Unity.Profiling
{
    public struct UnityProfiler : IProfiler
    {
        private readonly bool _calledProfiler;

        public UnityProfiler(string name, Thread unityThread)
        {
            if (Thread.CurrentThread == unityThread)
            {
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