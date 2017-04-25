namespace TycoonTerrain.TerrainGenerators
{
    public interface IGenerator<TOptions,TResult>
    {
        TResult Generate(TOptions options);
    }
}