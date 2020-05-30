using System;
using Votyra.Core.Logging;
using Votyra.Core.Models;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.Utils;

namespace Votyra.Core.Raycasting {
    public abstract class BaseCellRaycaster : IRaycasterPart {
        private const float MaxDistance = 500;

        private static readonly Vector2i Y1Offset = new Vector2i (0, 1);
        private static readonly Vector2i Y0Offset = new Vector2i (0, -1);
        private static readonly Vector2i X1Offset = new Vector2i (1, 0);
        private static readonly Vector2i X0Offset = new Vector2i (-1, 0);
        private readonly ITerrainVertexPostProcessor terrainVertexPostProcessor;
        private Vector2i fromCell;

        private Line2f raycastLine;
        private Vector2f rayDirection;
        private Vector2i toCell;

        protected BaseCellRaycaster (ITerrainVertexPostProcessor terrainVertexPostProcessor, IRaycasterAggregator raycasterAggregator = null) {
            this.terrainVertexPostProcessor = terrainVertexPostProcessor;
            raycasterAggregator?.Attach (this);
        }

        public virtual Vector3f Raycast (Ray3f cameraRay) {
            var cameraRayXy = cameraRay.XY ();
            var startXy = cameraRayXy.Origin;
            var directionNonNormalizedXy = cameraRay.Direction.XY ();
            var endXy = startXy + (directionNonNormalizedXy.Normalized () * MaxDistance);

            this.raycastLine = new Line2f (startXy, endXy);
            this.rayDirection = (endXy - startXy).Normalized ();
            this.fromCell = this.FindCell (startXy);
            this.toCell = this.FindCell (endXy);

            return this.RaycastGroup ();
        }

        public virtual float Raycast (Vector2f position) => this.RaycastCell (position);

        private Vector3f RaycastGroup () {
            var cell = this.fromCell;
            var position = this.raycastLine.From;
            var counter = 100;

            while ((cell != this.toCell) && (counter > 0)) {
                counter--;
                var ray = new Ray2f (position, this.rayDirection);

                var area = this.MeshCellArea (cell);
                var intersection = IntersectionUtils.LiangBarskyClipper (area, ray);
                if (intersection.AnyNan ()) {
                    return Vector3f.NaN;
                }

                var offset = IntersectionUtils.GetRectangleSegment (area, intersection);

                var stop = this.RaycastCell (new Line2f (position, intersection));
                if (!stop.AnyNan ()) {
                    return stop;
                }

                if (offset.HasFlag (RectangleSegment.X0)) {
                    cell += X0Offset;
                }

                if (offset.HasFlag (RectangleSegment.X1)) {
                    cell += X1Offset;
                }

                if (offset.HasFlag (RectangleSegment.Y0)) {
                    cell += Y0Offset;
                }

                if (offset.HasFlag (RectangleSegment.Y1)) {
                    cell += Y1Offset;
                }

                position = intersection;
            }

#if UNITY_EDITOR
            if (counter <= 0) {
                StaticLogger.LogError ("InvokeOnPath used too many iterations");
            }
#endif
            return Vector3f.NaN;
        }

        protected abstract Vector3f RaycastCell (Line2f line);

        protected abstract float RaycastCell (Vector2f point);

        private Vector2i FindCell (Vector2f meshPoint) {
            var cell = meshPoint.FloorToVector2i ();
            var area = this.MeshCellArea (cell);
            var counter = 100;
            while (!area.Contains (meshPoint) && (counter > 0)) {
                counter--;
                if (meshPoint.X > area.Max.X) {
                    cell += new Vector2i (1, 0);
                } else if (meshPoint.X < area.Min.X) {
                    cell -= new Vector2i (1, 0);
                } else if (meshPoint.Y > area.Max.Y) {
                    cell += new Vector2i (0, 1);
                } else if (meshPoint.Y < area.Min.Y) {
                    cell -= new Vector2i (0, 1);
                } else {
                    throw new InvalidOperationException ();
                }

                area = this.MeshCellArea (cell);
            }
#if UNITY_EDITOR
            if (counter <= 0) {
                StaticLogger.LogError ("FindCell used too many iterations");
            }
#endif

            return cell;
        }

        protected Area2f MeshCellArea (Vector2i cell) => Area2f.FromMinAndMax (this.ProcessVertex (new Vector2f (cell.X, cell.Y)), this.ProcessVertex (new Vector2f (cell.X + 1, cell.Y + 1)));

        private Vector2f ProcessVertex (Vector2f point) {
            if (this.terrainVertexPostProcessor == null) {
                return point;
            }

            return this.terrainVertexPostProcessor.PostProcessVertex (point.ToVector3f (0))
                .XY ();
        }
    }
}