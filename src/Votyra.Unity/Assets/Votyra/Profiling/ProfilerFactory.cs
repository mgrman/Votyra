using System;

namespace Votyra.Profiling
{
    public static class ProfilerFactory
    {
        public static Func<string, object, IProfiler> Factory { get; set; }

        public static IProfiler Create(string name, object owner)
        {
            return Factory(name, owner);
        }
    }
}