namespace Votyra.TerrainGenerators
{
    public interface IGenerator<TOptions,TResult>
    {
        TResult Generate(TOptions options);
    }
}