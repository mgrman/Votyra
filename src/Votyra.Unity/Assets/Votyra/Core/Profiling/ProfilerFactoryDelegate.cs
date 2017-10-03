using System;

namespace Votyra.Profiling
{
    public delegate IProfiler ProfilerFactoryDelegate(string name, object owner);

}