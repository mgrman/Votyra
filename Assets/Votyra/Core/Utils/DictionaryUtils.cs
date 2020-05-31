using System.Collections.Generic;

namespace Votyra.Core.Utils
{
    public static class DictionaryUtils
    {
        public static TValue TryGetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key)
            where TValue : class => dict.TryGetValue(key, out var temp) ? temp : null;

        public static TValue? TryGetValueN<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key)
            where TValue : struct => dict.TryGetValue(key, out var temp) ? temp : (TValue?)null;

        public static TValue TryRemoveAndReturnValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
            where TValue : class
        {
            if (dict.TryGetValue(key, out var temp))
            {
                dict.Remove(key);
                return temp;
            }

            return null;
        }

        public static TValue? TryRemoveAndReturnValueN<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
            where TValue : struct
        {
            if (dict.TryGetValue(key, out var temp))
            {
                dict.Remove(key);
                return temp;
            }

            return null;
        }
    }
}
