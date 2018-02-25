using System;

namespace Votyra.Core.Profiling
{
    public static class ProfilerFactoryExtensions
    {
        public static IProfiler Create<T>(this ProfilerFactoryDelegate factory)
        {
            return factory(typeof(T).FullName, null);
        }

        public static IProfiler Create(this ProfilerFactoryDelegate factory, Type type)
        {
            return factory(type.FullName, null);
        }

        public static IProfiler Create(this ProfilerFactoryDelegate factory, string name)
        {
            return factory(name, null);
        }

        public static IProfiler Create(this ProfilerFactoryDelegate factory, object instance)
        {
            return factory(instance.GetType().FullName, instance);
        }
    }
}