using System;

namespace Votyra.Logging
{
    public delegate IThreadSafeLogger LoggerFactoryDelegate(string name, object owner);
}