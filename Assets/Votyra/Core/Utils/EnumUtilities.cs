using System;
using System.Collections.Generic;
using System.Linq;
using Votyra.Core.InputHandling;

namespace Votyra.Core.Utils
{
    public static class EnumUtilities
    {
        public static IReadOnlyList<T> GetValues<T>() => ValueProvider<T>.Values;

        public static IReadOnlyList<string> GetNames<T>() => ValueProvider<T>.Names;

        public static IReadOnlyList<(T value, string name)> GetNamesAndValues<T>() => ValueProvider<T>.NamesAndValues;

        private static class ValueProvider<T>
        {
            public static readonly IReadOnlyList<T> Values = Enum.GetValues(typeof(T))
                .Cast<T>()
                .ToArray();

            public static readonly IReadOnlyList<string> Names = Values.Select(o => o.ToString())
                .ToArray();

            public static readonly IReadOnlyList<(T, string)> NamesAndValues = Values.Select(o => (o, o.ToString()))
                .ToArray();
        }
    }
}