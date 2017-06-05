using System;
using Votyra.Common.Models;
using Votyra.Images;
using Votyra.Unity.Images;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Votyra.Unity.GroupSelectors
{
    public class GroupVisibilityOptions : IDisposable
    {
        public readonly GameObject ParentContainer;
        public readonly Matrix4x4 ParentContainerLocalToWorldMatrix;
        public readonly Camera Camera;
        public readonly Matrix4x4 CameraTransform;
        public readonly Bounds GroupBounds;
        public readonly Range2i RangeZ;
        public readonly Rect2i InvalidatedArea;
        public readonly IEnumerable<Vector2i> ExistingGroups;
        public readonly Vector2i CellInGroupCount;

        public GroupVisibilityOptions(Camera camera, GameObject parentContainer, Range2i rangeZ, Rect2i invalidatedArea, Vector2i cellInGroupCount, IEnumerable<Vector2i> existingGroups)
        {
            this.Camera = camera;
            this.CameraTransform = this.Camera.cameraToWorldMatrix;
            this.ParentContainer = parentContainer;
            this.ParentContainerLocalToWorldMatrix = this.ParentContainer.transform.localToWorldMatrix;

            this.RangeZ = rangeZ;
            this.InvalidatedArea = invalidatedArea;
            this.CellInGroupCount = cellInGroupCount;
            this.ExistingGroups = existingGroups;

            this.GroupBounds = new Bounds(new Vector3(CellInGroupCount.x / 2.0f, CellInGroupCount.y / 2.0f, RangeZ.Center), new Vector3(CellInGroupCount.x, CellInGroupCount.y, RangeZ.Size));
        }

        public bool IsChanged(GroupVisibilityOptions old)
        {
            return old == null ||
                this.RangeZ != old.RangeZ ||
                this.InvalidatedArea != old.InvalidatedArea ||
                this.ParentContainer != old.ParentContainer ||
                this.ParentContainerLocalToWorldMatrix != old.ParentContainerLocalToWorldMatrix ||
                this.CellInGroupCount != old.CellInGroupCount ||
                (this.ExistingGroups ?? Enumerable.Empty<Vector2i>()).SequenceEqual(old.ExistingGroups ?? Enumerable.Empty<Vector2i>()) ||
                this.Camera != old.Camera ||
                this.CameraTransform != old.CameraTransform;
        }

        public void Dispose()
        {
        }
    }
}