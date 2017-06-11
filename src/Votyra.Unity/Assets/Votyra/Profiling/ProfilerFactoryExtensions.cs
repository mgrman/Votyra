using System;

namespace Votyra.Profiling
{
    public static class ProfilerFactoryExtensions
    {
        public static IProfiler CreateProfiler(this object owner, string name)
        {
            return ProfilerFactory.Create(name, owner);
        }
    }
}