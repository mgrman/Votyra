using System;
using System.Diagnostics;
using UniRx;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;
using Thread = System.Threading.Thread;

namespace Votyra.Core.Profiling
{
    public class UnityProfiler : IProfiler
    {
        private readonly Object owner;

        private readonly Stopwatch stopwatch;

        private string name;

        public UnityProfiler(Object owner)
        {
            this.owner = owner;
            this.stopwatch = new Stopwatch();
        }

        public IDisposable Start(string name)
        {
            this.name = name;
            if (Thread.CurrentThread == UnitySyncContext.UnityThread)
            {
                if (this.owner != null)
                {
                    Profiler.BeginSample(name, this.owner);
                }
                else
                {
                    Profiler.BeginSample(name);
                }

                this.stopwatch.Start();
                return Disposable.Create(this.StopAndEndSample);
            }

            this.stopwatch.Start();
            return Disposable.Create(this.Stop);
        }

        private void StopAndEndSample()
        {
            this.Stop();
            Profiler.EndSample();
        }

        private void Stop()
        {
            this.stopwatch.Stop();
            UnityProfilerAggregator.Add(this.owner, this.name, (double)this.stopwatch.ElapsedTicks / TimeSpan.TicksPerMillisecond);
            this.stopwatch.Reset();
            // UnityEngine.Debug.Log(_namePrefix + _stopwatch.ElapsedMilliseconds);
        }
    }
}