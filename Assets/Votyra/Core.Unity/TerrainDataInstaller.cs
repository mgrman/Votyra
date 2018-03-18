using UnityEngine;
using Votyra.Core.Images;
using Votyra.Core.MeshUpdaters;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Votyra.Core.GroupSelectors;
using Votyra.Core.Images;
using Votyra.Core.Images.Constraints;
using Votyra.Core.ImageSamplers;
using Votyra.Core.TerrainGenerators;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Zenject;

namespace Votyra.Core.Unity
{
    public class TerrainDataInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TerrainManagerModel>().AsSingle();
            Container.Bind<ITerrainConfig>().FromMethod((injectContext) =>
            {
                var model = injectContext.Container.Resolve<ITerrainManagerModel>();
                return model.TerrainConfig.Value;
            });
            Container.Bind<IInitialImageConfig>().FromMethod((injectContext) =>
            {
                var model = injectContext.Container.Resolve<ITerrainManagerModel>();
                return model.InitialImageConfig.Value;
            });
            Container.Bind<IImageConfig>().FromMethod((injectContext) =>
            {
                var model = injectContext.Container.Resolve<ITerrainManagerModel>();
                return model.ImageConfig.Value;
            });
        }
    }
}