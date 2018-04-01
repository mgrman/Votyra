using System.Collections.Generic;

namespace Votyra.Core.Utils
{
    public static class DictionaryUtils
    {
        public static TValue TryGetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key)
            where TValue : class
        {
            TValue temp;
            return dict.TryGetValue(key, out temp) ? temp : null;
        }

        public static TValue? TryGetValueN<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key)
            where TValue : struct
        {
            TValue temp;
            return dict.TryGetValue(key, out temp) ? temp : (TValue?)null;
        }
    }
}