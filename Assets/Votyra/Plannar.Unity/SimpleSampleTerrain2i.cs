using Votyra.Plannar.GroupSelectors;
using Votyra.Plannar.Images;
using Votyra.Plannar.Images.Constraints;
using Votyra.Plannar.ImageSamplers;
using Votyra.Plannar.MeshUpdaters;
using Votyra.Plannar.TerrainGenerators;
using Votyra.Plannar.TerrainGenerators.TerrainMeshers;

namespace Votyra.Plannar
{
    //TODO: move to floats
    public class SimpleSampleTerrain2i : TerrainGeneratorBehaviour2i
    {
        protected override void Initialize()
        {
            _sampler = new SimpleImageSampler2i();
            _editableImageConstraint = new SimpleTycoonTileConstraint2i(_sampler);

            _terrainGenerator = new TerrainGenerator2i<TerrainMesher2i>();
            _meshUpdater = new TerrainMeshUpdater2i();
            _groupsSelector = new GroupsByCameraVisibilitySelector2i();
        }
    }
}