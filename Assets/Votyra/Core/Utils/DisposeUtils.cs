using System;

namespace Votyra.Core.Utils
{
    public static class DisposeUtils
    {
        public static void TryDispose(this object o)
        {
            (o as IDisposable)?.Dispose();
        }
    }
}
