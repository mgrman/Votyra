using System.Threading;
using TycoonTerrain.Common.Profiling;
using UnityEngine.Profiling;

namespace TycoonTerrain.Unity.Profiling
{
    public class UnityProfiler : IProfiler
    {
        private readonly bool _calledProfiler;

        public UnityProfiler(string name, Thread unityThread)
        {
            if (Thread.CurrentThread == unityThread)
            {
                Profiler.BeginSample(name);
                _calledProfiler = true;
            }
        }

        public void Dispose()
        {
            if (_calledProfiler)
                Profiler.EndSample();
        }
    }
}