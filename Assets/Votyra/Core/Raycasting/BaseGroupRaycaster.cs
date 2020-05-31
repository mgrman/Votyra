using System;
using Votyra.Core.Logging;
using Votyra.Core.Models;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.Utils;

namespace Votyra.Core.Raycasting
{
    public abstract class BaseGroupRaycaster : IRaycasterPart
    {
        private readonly Vector2i cellInGroupCount;
        private readonly ITerrainRepository2i manager;
        private readonly float maxDistance = 500; // TODO get from camera
        private readonly int maxIterations;
        private readonly ITerrainVertexPostProcessor terrainVertexPostProcessor;

        public BaseGroupRaycaster(ITerrainConfig terrainConfig, ITerrainVertexPostProcessor terrainVertexPostProcessor = null, IRaycasterAggregator raycasterAggregator = null)
        {
            this.terrainVertexPostProcessor = terrainVertexPostProcessor;
            raycasterAggregator?.Attach(this);
            this.cellInGroupCount = terrainConfig.CellInGroupCount.XY();

            this.maxIterations = (int)this.maxDistance * 10;
        }

        public virtual float Raycast(Vector2f posXy)
        {
            var group = this.GetGroup(this.FindCell(posXy));
            return this.RaycastGroup(group, posXy);
        }

        public virtual Vector3f Raycast(Ray3f cameraRay)
        {
            var cameraRayXy = cameraRay.XY();

            var rayOriginXy = cameraRayXy.Origin;
            var rayDirectionXy = cameraRayXy.Direction;
            var maxDistancePoint = cameraRay.GetPoint(this.maxDistance)
                .XY();

            var fromGroup = this.GetGroup(this.FindCell(rayOriginXy));
            var toGroup = this.GetGroup(this.FindCell(maxDistancePoint));

            var currentGroup = fromGroup;
            var currentPosition = rayOriginXy;
            var currentCounter = this.maxIterations;
            while ((currentGroup != toGroup) && (currentCounter > 0))
            {
                currentCounter--;
                var ray = new Ray2f(currentPosition, rayDirectionXy);

                var area = this.MeshGroupArea(currentGroup);
                var foundResult = this.RaycastGroup(currentGroup, cameraRay);
                if (foundResult.NoNan())
                {
                    return foundResult;
                }

                var intersection = IntersectionUtils.LiangBarskyClipper(area, ray);
                if (intersection.AnyNan())
                {
                    return Vector3f.NaN;
                }

                var offset = IntersectionUtils.GetRectangleSegment(area, intersection);
                if (offset.IsInSegment(RectangleSegment.X0))
                {
                    currentGroup += Vector2iUtils.MinusOneX;
                }

                if (offset.IsInSegment(RectangleSegment.X1))
                {
                    currentGroup += Vector2iUtils.PlusOneX;
                }

                if (offset.IsInSegment(RectangleSegment.Y0))
                {
                    currentGroup += Vector2iUtils.MinusOneY;
                }

                if (offset.IsInSegment(RectangleSegment.Y1))
                {
                    currentGroup += Vector2iUtils.PlusOneY;
                }

                currentPosition = intersection;
            }

#if UNITY_EDITOR
            if (currentCounter <= 0)
            {
                StaticLogger.LogError("InvokeOnPath used too many iterations");
            }
#endif
            return Vector3f.NaN;
        }

        private Vector2i FindCell(Vector2f meshPoint)
        {
            var cell = meshPoint.FloorToVector2i();
            var area = this.MeshCellArea(cell);
            var counter = 100;
            while (!area.Contains(meshPoint) && (counter > 0))
            {
                counter--;
                if (meshPoint.X > area.Max.X)
                {
                    cell += new Vector2i(1, 0);
                }
                else if (meshPoint.X < area.Min.X)
                {
                    cell -= new Vector2i(1, 0);
                }
                else if (meshPoint.Y > area.Max.Y)
                {
                    cell += new Vector2i(0, 1);
                }
                else if (meshPoint.Y < area.Min.Y)
                {
                    cell -= new Vector2i(0, 1);
                }
                else
                {
                    throw new InvalidOperationException();
                }

                area = this.MeshCellArea(cell);
            }
#if UNITY_EDITOR
            if (counter <= 0)
            {
                StaticLogger.LogError("FindCell used too many iterations");
            }
#endif

            return cell;
        }

        private Area2f MeshCellArea(Vector2i cell) => Area2f.FromMinAndMax(this.ProcessVertex(new Vector2f(cell.X, cell.Y)), this.ProcessVertex(new Vector2f(cell.X + 1, cell.Y + 1)));

        private Area2f MeshGroupArea(Vector2i group)
        {
            var min = this.MeshCellArea(group * this.cellInGroupCount)
                .Min;

            var max = this.MeshCellArea(((group + Vector2i.One) * this.cellInGroupCount) - Vector2i.One)
                .Max;

            return Area2f.FromMinAndMax(min, max);
        }

        private Vector2f ProcessVertex(Vector2f point)
        {
            if (this.terrainVertexPostProcessor == null)
            {
                return point;
            }

            return this.terrainVertexPostProcessor.PostProcessVertex(point.ToVector3f(0))
                .XY();
        }

        private Vector2i GetGroup(Vector2i cell)
        {
            var x = (cell.X / this.cellInGroupCount.X) - ((cell.X < 0) && ((cell.X % this.cellInGroupCount.X) != 0) ? 1 : 0);
            var y = (cell.Y / this.cellInGroupCount.Y) - ((cell.Y < 0) && ((cell.Y % this.cellInGroupCount.Y) != 0) ? 1 : 0);
            return new Vector2i(x, y);
        }

        protected abstract Vector3f RaycastGroup(Vector2i group, Ray3f cameraRay);

        protected abstract float RaycastGroup(Vector2i group, Vector2f posXy);
    }
}
