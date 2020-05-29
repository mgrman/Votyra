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
        private readonly bool _calledProfiler;

        private readonly Object _owner;

        private readonly Stopwatch _stopwatch;

        private string _name;

        public UnityProfiler(Object owner)
        {
            this._owner = owner;
            this._stopwatch = new Stopwatch();
        }

        public IDisposable Start(string name)
        {
            this._name = name;
            if (Thread.CurrentThread == UnitySyncContext.UnityThread)
            {
                if (this._owner != null)
                {
                    Profiler.BeginSample(name, this._owner);
                }
                else
                {
                    Profiler.BeginSample(name);
                }

                this._stopwatch.Start();
                return Disposable.Create(this.StopAndEndSample);
            }

            this._stopwatch.Start();
            return Disposable.Create(this.Stop);
        }

        private void StopAndEndSample()
        {
            this.Stop();
            Profiler.EndSample();
        }

        private void Stop()
        {
            this._stopwatch.Stop();
            UnityProfilerAggregator.Add(this._owner, this._name, (double)this._stopwatch.ElapsedTicks / TimeSpan.TicksPerMillisecond);
            this._stopwatch.Reset();
            // UnityEngine.Debug.Log(_namePrefix + _stopwatch.ElapsedMilliseconds);
        }
    }
}
