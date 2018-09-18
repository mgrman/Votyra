using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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