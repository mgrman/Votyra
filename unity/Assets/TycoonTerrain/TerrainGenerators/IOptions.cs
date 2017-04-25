using System;

namespace TycoonTerrain.TerrainGenerators
{
    public interface IOptions<T> : IOptions
    {
        bool IsChanged(T options);
        new T Clone();
    }

    public interface IOptions : IDisposable
    {
        bool IsValid { get; }
        bool IsChanged(IOptions options);
        IOptions Clone();
    }
}