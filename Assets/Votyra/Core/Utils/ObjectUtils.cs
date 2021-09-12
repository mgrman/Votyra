using System;

namespace Votyra.Core.Utils
{
    public static class ObjectUtils
    {
        public static void UpdateType<T, TR>(ref TR property) where T : TR, new()
        {
            if (!(property is T))
            {
                if (property == null)
                {
                    //Debug.Log("Creating new AsyncTerainGeneratorService");
                    property = new T();
                }
                else if (!(property is T))
                {
                    //Debug.Log("Overriding with AsyncTerainGeneratorService");
                    if (property is IDisposable)
                        (property as IDisposable).Dispose();
                    property = new T();
                }
            }
        }
    }
}