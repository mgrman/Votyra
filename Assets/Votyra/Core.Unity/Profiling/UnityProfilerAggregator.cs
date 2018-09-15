using System;
using System.Collections.Generic;

namespace Votyra.Core.Profiling
{
    public static class UnityProfilerAggregator
    {
        private static object _lock = new Object();
        private static Dictionary<Tuple<UnityEngine.Object, string>, double> Values { get; } = new Dictionary<Tuple<UnityEngine.Object, string>, double>();

        public static void Add(UnityEngine.Object owner, string name, double ms)
        {
            lock (_lock)
            {
                Values[Tuple.Create(owner, name)] = ms;
            }
        }

        public static IReadOnlyDictionary<Tuple<UnityEngine.Object, string>, double> ValuesClone()
        {
            Dictionary<Tuple<UnityEngine.Object, string>, double> clone;
            lock (_lock)
            {
                clone = new Dictionary<Tuple<UnityEngine.Object, string>, double>(Values);
            }
            return clone;
        }
    }
}