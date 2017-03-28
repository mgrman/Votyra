using System;
using TycoonTerrain.Images;
using TycoonTerrain.Unity.Images;
using TycoonTerrain.Unity.Utils;
using UnityEngine;

namespace TycoonTerrain.Unity.GroupSelectors
{
    public class GroupVisibilityOptions : IDisposable
    {
        public readonly GameObject ParentContainer;
        public readonly Camera Camera;
        public readonly Matrix4x4 CameraTransform;
        public readonly Bounds GroupBounds;
        public readonly TycoonTerrain.Common.Models.Range2i RangeZ;

        public readonly TycoonTerrain.Common.Models.Vector2i CellInGroupCount;
        private readonly IImage2i Image;

        public GroupVisibilityOptions(TerrainGeneratorBehaviour terrainGenerator,Camera camera)
        {
            this.Camera = camera;
            this.CameraTransform = this.Camera.cameraToWorldMatrix;
            this.ParentContainer = terrainGenerator.gameObject;

            IImage2i image = (terrainGenerator.Image as IImage2iProvider)?.Image;
            this.Image = image;
            this.RangeZ = Image.RangeZ;
            this.CellInGroupCount = terrainGenerator.CellInGroupCount.ToDomain();

            this.GroupBounds = new Bounds(new Vector3(CellInGroupCount.x / 2.0f, CellInGroupCount.y / 2.0f, RangeZ.Center), new Vector3(CellInGroupCount.x, CellInGroupCount.y, RangeZ.Size));
        }

        public bool IsChanged(GroupVisibilityOptions old)
        {
            //TODO
            //not complete check!
            return old == null ||
                this.Image != old.Image ||
                this.ParentContainer != old.ParentContainer ||
                this.RangeZ != old.RangeZ ||
                this.Camera != old.Camera ||
                this.CameraTransform != old.CameraTransform;
        }

        public bool IsValid
        {
            get
            {
                return this.ParentContainer != null;
            }
        }

        public void Dispose()
        {
        }
    }
}