using System;
using Votyra.Core.Images;
using Votyra.Core.Logging;
using Votyra.Core.Models;
using Votyra.Core.TerrainGenerators.TerrainMeshers;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;

namespace Votyra.Core.Raycasting
{
    public abstract class BaseGroupRaycaster : IRaycaster
    {
        private readonly float maxDistance = 500; //TODO get from camera
        private readonly int maxIterations;
        private readonly ITerrainVertexPostProcessor _terrainVertexPostProcessor;
        private readonly Vector2i _cellInGroupCount;
        private readonly ITerrainGeneratorManager2i _manager;

        public BaseGroupRaycaster(ITerrainConfig terrainConfig, ITerrainVertexPostProcessor terrainVertexPostProcessor = null)
        {
            _terrainVertexPostProcessor = terrainVertexPostProcessor;
            _cellInGroupCount = terrainConfig.CellInGroupCount.XY();

            maxIterations = (int) maxDistance * 10;
        }

        public virtual Vector3f Raycast(Ray3f cameraRay)
        {
            var cameraRayXY = cameraRay.XY();
            
            var rayOriginXY = cameraRayXY.Origin;
            var rayDirectionXY = cameraRayXY.Direction;
            var maxDistancePoint = cameraRay.GetPoint(maxDistance)
                .XY();

            var fromGroup = GetGroup(FindCell(rayOriginXY));
            var toGroup = GetGroup(FindCell(maxDistancePoint));

            var currentGroup = fromGroup;
            var currentPosition = rayOriginXY;
            var currentCounter = maxIterations;
            while (currentGroup != toGroup && currentCounter > 0)
            {
                currentCounter--;
                var ray = new Ray2f(currentPosition, rayDirectionXY);

                var area = MeshGroupArea(currentGroup);
                if (!LiangBarskyClipper.Compute(area, ray, out var intersection, out var offset))
                    return Vector3f.NaN;

                var foundResult = RaycastGroup(intersection, currentGroup, cameraRay);
                switch (foundResult.State)
                {
                    case RaycastResultState.Success:
                        return foundResult.Hit;
                    case RaycastResultState.FullStop:
                        return Vector3f.NaN;
                }

                if (offset.HasFlag(Side.X0))
                {
                    currentGroup += Vector2iUtils.MinusOneX;
                }

                if (offset.HasFlag(Side.X1))
                {
                    currentGroup += Vector2iUtils.PlusOneX;
                }

                if (offset.HasFlag(Side.Y0))
                {
                    currentGroup += Vector2iUtils.MinusOneY;
                }

                if (offset.HasFlag(Side.Y1))
                {
                    currentGroup += Vector2iUtils.PlusOneY;
                }

                currentPosition = intersection.To;
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
            var area = MeshCellArea(cell);
            var counter = 100;
            while (!area.Contains(meshPoint) && counter > 0)
            {
                counter--;
                if (meshPoint.X > area.Max.X)
                    cell += new Vector2i(1, 0);
                else if (meshPoint.X < area.Min.X)
                    cell -= new Vector2i(1, 0);
                else if (meshPoint.Y > area.Max.Y)
                    cell += new Vector2i(0, 1);
                else if (meshPoint.Y < area.Min.Y)
                    cell -= new Vector2i(0, 1);
                else
                    throw new InvalidOperationException();

                area = MeshCellArea(cell);
            }
#if UNITY_EDITOR
            if (counter <= 0)
            {
                StaticLogger.LogError("FindCell used too many iterations");
            }
#endif

            return cell;
        }

        private Area2f MeshCellArea(Vector2i cell) => Area2f.FromMinAndMax(ProcessVertex(new Vector2f(cell.X, cell.Y)), ProcessVertex(new Vector2f(cell.X + 1, cell.Y + 1)));

        private Area2f MeshGroupArea(Vector2i group) =>
            Area2f.FromMinAndMax(MeshCellArea(group * _cellInGroupCount)
                    .Min,
                MeshCellArea((group + Vector2i.One) * _cellInGroupCount - Vector2i.One)
                    .Max);

        private Vector2f ProcessVertex(Vector2f point)
        {
            if (_terrainVertexPostProcessor == null)
                return point;
            return _terrainVertexPostProcessor.PostProcessVertex(point.ToVector3f(0))
                .XY();
        }

        private Vector2i GetGroup(Vector2i cell)
        {
            var x = cell.X / _cellInGroupCount.X - ((cell.X < 0 && cell.X % _cellInGroupCount.X != 0) ? 1 : 0);
            var y = cell.Y / _cellInGroupCount.Y - ((cell.Y < 0 && cell.Y % _cellInGroupCount.Y != 0) ? 1 : 0);
            return new Vector2i(x, y);
        }

        protected abstract RaycastResult RaycastGroup(Line2f line, Vector2i group, Ray3f cameraRay);

        protected enum RaycastResultState
        {
            NoHit = 0,
            Success = 1,
            FullStop = 2
        }

        protected struct RaycastResult
        {
            private RaycastResult(Vector3f hit, RaycastResultState state)
            {
                Hit = hit;
                State = state;
            }

            public RaycastResult(Vector3f hit)
            {
                Hit = hit;
                State = RaycastResultState.Success;
            }

            public readonly Vector3f Hit;
            public readonly RaycastResultState State;

            public static readonly RaycastResult NoHit = new RaycastResult(Vector3f.NaN, RaycastResultState.NoHit);
            public static readonly RaycastResult FullStop = new RaycastResult(Vector3f.NaN, RaycastResultState.FullStop);
        }
    }
}