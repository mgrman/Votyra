using System;

namespace Votyra.Core.Logging
{
    public delegate IThreadSafeLogger LoggerFactoryDelegate(string name, object owner);
}
