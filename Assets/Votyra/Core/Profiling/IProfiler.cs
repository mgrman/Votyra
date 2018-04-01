using System;

namespace Votyra.Core.Profiling
{
    public interface IProfiler
    {
        IDisposable Start(string name);
    }
}