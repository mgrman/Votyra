namespace Votyra.Models
{
    public interface IMatrix<T>
    {
        T this[Vector2i i] { get; set; }
        T this[int ix, int iy] { get; set; }
    }
}