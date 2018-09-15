using System;
using System.Diagnostics;
using System.Threading;
using UniRx;
using UnityEngine.Profiling;

namespace Votyra.Core.Profiling
{
    public class UnityProfiler : IProfiler
    {
        private readonly bool _calledProfiler;

        private readonly UnityEngine.Object _owner;

        private readonly Stopwatch _stopwatch;

        private string _name;

        public UnityProfiler(UnityEngine.Object owner)
        {
            _owner = owner;
            _stopwatch = new Stopwatch();
        }

        public IDisposable Start(string name)
        {
            _name = name;
            if (Thread.CurrentThread == UnitySyncContext.UnityThread)
            {
                if (_owner != null)
                    Profiler.BeginSample(name, _owner);
                else
                    Profiler.BeginSample(name);

                _stopwatch.Start();
                return Disposable.Create(StopAndEndSample);
            }
            else
            {
                _stopwatch.Start();
                return Disposable.Create(Stop);
            }
        }

        private void StopAndEndSample()
        {
            Stop();
            Profiler.EndSample();
        }

        private void Stop()
        {
            _stopwatch.Stop();
            UnityProfilerAggregator.Add(_owner, _name, (double)_stopwatch.ElapsedTicks / TimeSpan.TicksPerMillisecond);
            _stopwatch.Reset();
            // UnityEngine.Debug.Log(_namePrefix + _stopwatch.ElapsedMilliseconds);
        }
    }
}