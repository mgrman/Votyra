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
            _editableImageConstraint = new SimpleTycoonTileConstraint2i();
            _sampler = new SimpleImageSampler2i();

            _terrainGenerator = new TerrainGenerator2i<TerrainMesher2i>();
            _meshUpdater = new TerrainMeshUpdater2i();
            _groupsSelector = new GroupsByCameraVisibilitySelector2i();
            _imageProvider = new EditableMatrixImage2f(InitialTexture, InitialTextureScale, _sampler, _editableImageConstraint);
        }
    }
}