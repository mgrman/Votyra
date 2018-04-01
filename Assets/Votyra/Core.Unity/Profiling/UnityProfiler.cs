using System;
using System.Threading;

using UnityEngine.Profiling;

namespace Votyra.Core.Profiling
{
    public class UnityProfiler : IProfiler
    {
        private readonly bool _calledProfiler;

        private readonly UnityEngine.Object _owner;

        public UnityProfiler(UnityEngine.Object owner)
        {
            _owner = owner;
        }

        public IDisposable Start(string name)
        {
            if (Thread.CurrentThread == UnitySyncContext.UnityThread)
            {
                if (_owner != null)
                    Profiler.BeginSample(name, _owner);
                else
                    Profiler.BeginSample(name);
                return new EndSampleDisposable();
            }
            else
            {
                return new EmptyDisposable();
            }
        }

        private struct EmptyDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }

        private struct EndSampleDisposable : IDisposable
        {
            public void Dispose()
            {
                Profiler.EndSample();
            }
        }
    }
}