using System;
using UnityEngine;
using System.Collections.Generic;

namespace Votyra.Core.Models
{
    public interface IReadOnlySet<T> : IEnumerable<T>
    {
        bool Contains(T value);
        int Count { get; }
    }
}
