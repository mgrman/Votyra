using System;

namespace Votyra.Core.Models
{
    public interface IMatrix3<T>
    {
        Vector3i Size { get; }
        T this[Vector3i i] { get; set; }
    }
}