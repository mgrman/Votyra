using System;
using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.Pooling
{
    public interface IReadOnlyPooledSet<T> : IReadOnlySet<T>, IDisposable { }
}