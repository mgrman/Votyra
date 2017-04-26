using System;
using TycoonTerrain.Common.Models;
using TycoonTerrain.Images;
using TycoonTerrain.Unity.Images;
using UnityEngine;

namespace TycoonTerrain.Unity.GroupSelectors
{
    public class GroupVisibilityOptions : IDisposable
    {
        public readonly GameObject ParentContainer;
        public readonly Camera Camera;
        public readonly Matrix4x4 CameraTransform;
        public readonly Bounds GroupBounds;
        public readonly Range2i RangeZ;

        public readonly Vector2i CellInGroupCount;

        public GroupVisibilityOptions( Camera camera,GameObject parentContainer, Range2i rangeZ, Vector2i cellInGroupCount)
        {
            this.Camera = camera;
            this.CameraTransform = this.Camera.cameraToWorldMatrix;
            this.ParentContainer = parentContainer;
            
            this.RangeZ = rangeZ;
            this.CellInGroupCount = cellInGroupCount;

            this.GroupBounds = new Bounds(new Vector3(CellInGroupCount.x / 2.0f, CellInGroupCount.y / 2.0f, RangeZ.Center), new Vector3(CellInGroupCount.x, CellInGroupCount.y, RangeZ.Size));
        }


        public bool IsChanged(GroupVisibilityOptions old)
        {
            //TODO
            //not complete check!
            return old == null ||
                this.RangeZ != old.RangeZ ||
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