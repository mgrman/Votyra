using System.Collections.Generic;

namespace Votyra.Core.Models
{
    public interface IReadOnlySet<T> : IEnumerable<T>
    {
        int Count { get; }

        bool Contains(T value);
    }
}