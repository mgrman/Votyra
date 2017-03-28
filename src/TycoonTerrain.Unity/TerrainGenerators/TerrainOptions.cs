using System.Collections.Generic;
using TycoonTerrain.Images;
using TycoonTerrain.ImageSamplers;
using TycoonTerrain.TerrainAlgorithms;
using TycoonTerrain.TerrainMeshers;
using TycoonTerrain.Unity;
using TycoonTerrain.Unity.Images;
using TycoonTerrain.Unity.Utils;

namespace TycoonTerrain.Unity.TerrainGenerators
{
    public static class TerrainOptionsFactory
    {
        public static TycoonTerrain.TerrainGenerators.TerrainOptions Create(TerrainGeneratorBehaviour terrainGenerator, IList<TycoonTerrain.Common.Models.Vector2i> groupsToUpdate)
        {
            IImage2i image = (terrainGenerator.Image as IImage2iProvider)?.Image;
            
            return new TycoonTerrain.TerrainGenerators.TerrainOptions(terrainGenerator.CellInGroupCount.ToDomain(), terrainGenerator.FlipTriangles, image, terrainGenerator.Sampler, terrainGenerator.MeshGenerator, terrainGenerator.TerrainMesher, UnityEngine.Time.time, groupsToUpdate);
        }
    }
}