using System.Collections.Generic;

namespace Votyra.Core.Utils
{
    public static class DictionaryUtils
    {
        public static TValue TryRemoveAndReturnValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : class
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