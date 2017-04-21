using System.Collections.Generic;
using TycoonTerrain.Common.Models;
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
            IImage2iProvider imageProvider = terrainGenerator.Image as IImage2iProvider;
            IImage2i image = imageProvider==null?null: imageProvider.Image;

            Vector2i cellInGroupCount = new Vector2i(terrainGenerator.CellInGroupCount.x, terrainGenerator.CellInGroupCount.y);
            return new TycoonTerrain.TerrainGenerators.TerrainOptions(cellInGroupCount, terrainGenerator.FlipTriangles, image, terrainGenerator.Sampler, terrainGenerator.MeshGenerator, terrainGenerator.TerrainMesher, UnityEngine.Time.time, groupsToUpdate);
        }
    }
}