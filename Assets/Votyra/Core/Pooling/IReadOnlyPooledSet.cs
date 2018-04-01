using System;
using Votyra.Core.Models;

namespace Votyra.Core.Pooling
{
    public interface IReadOnlyPooledSet<T> : IReadOnlySet<T>, IDisposable { }
}