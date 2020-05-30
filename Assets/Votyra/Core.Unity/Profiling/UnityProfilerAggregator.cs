using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Votyra.Core.Profiling
{
    public static class UnityProfilerAggregator
    {
        private static readonly object Lock = new object();

        private static Dictionary<System.Tuple<Object, string>, double> Values { get; } = new Dictionary<System.Tuple<Object, string>, double>();

        public static void Add(Object owner, string name, double ms)
        {
            lock (Lock)
            {
                Values[Tuple.Create(owner, name)] = ms;
            }
        }

        public static IReadOnlyDictionary<System.Tuple<Object, string>, double> ValuesClone()
        {
            Dictionary<Tuple<Object, string>, double> clone;
            lock (Lock)
            {
                clone = new Dictionary<Tuple<Object, string>, double>(Values);
            }

            return clone;
        }
    }
}
