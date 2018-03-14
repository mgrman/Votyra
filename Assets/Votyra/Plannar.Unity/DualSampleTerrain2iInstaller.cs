using UnityEngine;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Votyra.Plannar.GroupSelectors;
using Votyra.Plannar.Images;
using Votyra.Plannar.Images.Constraints;
using Votyra.Plannar.ImageSamplers;
using Votyra.Plannar.TerrainGenerators;
using Votyra.Plannar.TerrainGenerators.TerrainMeshers;
using Zenject;

namespace Votyra.Plannar.Unity
{
    public class DualSampleTerrain2iInstaller : MonoInstaller
    {

        public override void InstallBindings()
        {
            Container.Rebind<IImageSampler2i>().To<DualImageSampler2i>().AsSingle();
            Container.Rebind<IImageConstraint2i>().To<DualSampledTycoonTileConstraint2i>().AsSingle();
        }

    }
}