using UnityEngine;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Votyra.Core.GroupSelectors;
using Votyra.Core.ImageSamplers;
using Votyra.Core.TerrainGenerators;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Zenject;
using Votyra.Core.Images.Constraints;

namespace Votyra.Cubical.Unity
{
    public class SimpleSampleTerrain3bInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Rebind<IImageSampler3>().To<SimpleImageSampler3b>().AsSingle();
            // Container.Rebind<IImageConstraint3i>().To<BooleanConstraint3i>().AsSingle();
            Container.Rebind<ITerrainMesher3b>().To<TerrainMesher3b>().AsSingle();
        }
    }
}