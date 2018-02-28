namespace Votyra.Core.Models
{
    public interface IMatrix<T>
    {
        Vector2i size { get; }
        T this[Vector2i i] { get; set; }
        T this[int ix, int iy] { get; set; }
    }
}