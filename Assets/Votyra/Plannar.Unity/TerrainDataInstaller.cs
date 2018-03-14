using UnityEngine;
using Votyra.Core.Images;
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
    public class TerrainDataInstaller : MonoInstaller
    {
        [SerializeField]
        private UI_Vector3i _imageSize;

        [SerializeField]
        private UnityEngine.Object _initialData;

        [SerializeField]
        private Vector3 _initialDataScale;

        [SerializeField]
        private UI_Vector2i _cellInGroupCount = new UI_Vector2i(10, 10);

        [SerializeField]
        private bool _flipTriangles;

        [SerializeField]
        private bool _drawBounds;

        [SerializeField]
        private bool _async = true;

        [SerializeField]
        private Material _material;

        [SerializeField]
        private Material _materialWalls;

        public ITerrainConfig TerrainConfig
        {
            get
            {
                return new TerrainConfig(_imageSize, _initialData, _initialDataScale.ToVector3f(), _cellInGroupCount, _flipTriangles, _drawBounds, _async, _material, _materialWalls);
            }
        }

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ITerrainConfig>().FromInstance(TerrainConfig);
        }


    }
}