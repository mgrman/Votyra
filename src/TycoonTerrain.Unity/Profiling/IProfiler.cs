using System;
using TycoonTerrain.Common.Profiling;
using UnityEngine.Profiling;

namespace TycoonTerrain.Unity.Profiling
{
    public class UnityProfiler : IProfiler
    {
        public UnityProfiler(string name)
        {
            Profiler.BeginSample(name);
        }

        public void Dispose()
        {
            Profiler.EndSample();
        }
    }
}