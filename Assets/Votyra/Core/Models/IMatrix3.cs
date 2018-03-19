namespace Votyra.Core.Models
{
    public interface IMatrix3<T>
    {
        Vector3i size { get; }
        T this[Vector3i i] { get; set; }
        T this[int ix, int iy, int iz] { get; set; }
    }
}