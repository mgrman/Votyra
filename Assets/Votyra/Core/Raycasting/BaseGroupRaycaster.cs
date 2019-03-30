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
        private const float maxDistance = 500;

        private static readonly Vector2i Y1Offset = new Vector2i(0, 1);
        private static readonly Vector2i Y0Offset = new Vector2i(0, -1);
        private static readonly Vector2i X1Offset = new Vector2i(1, 0);
        private static readonly Vector2i X0Offset = new Vector2i(-1, 0);
        private readonly ITerrainVertexPostProcessor _terrainVertexPostProcessor;
        private readonly Vector2i _cellInGroupCount;

        private Line2f _raycastLine;
        private Vector2f _rayDirection;
        private Vector2i _fromGroup;
        private Vector2i _toGroup;
        private IImage2f _image;
        private readonly ITerrainGeneratorManager2i _manager;
        private IMask2e _mask;
        private bool _wasValidMesh;

        public BaseGroupRaycaster(ITerrainConfig terrainConfig, ITerrainVertexPostProcessor terrainVertexPostProcessor = null)
        {
            _terrainVertexPostProcessor = terrainVertexPostProcessor;
            _cellInGroupCount = terrainConfig.CellInGroupCount.XY();
        }

        public virtual Vector3f Raycast(Ray3f cameraRay)
        {
            _wasValidMesh = false;
            
            var cameraRayXY = cameraRay.XY();
            var startXY = cameraRayXY.Origin;
            var directionNonNormalizedXY = cameraRay.Direction.XY();
            var endXY = startXY + directionNonNormalizedXY.Normalized() * maxDistance;

            _raycastLine = new Line2f(startXY, endXY);
            _rayDirection = (endXY - startXY).Normalized();
            _fromGroup = GetGroup(FindCell(startXY));
            _toGroup = GetGroup(FindCell(endXY));

            return RaycastGroup();
        }

        private Vector3f RaycastGroup()
        {
            var group = _fromGroup;
            var position = _raycastLine.From;
            var counter = 100;

            while (group != _toGroup && counter > 0)
            {
                counter--;
                var ray = new Ray2f(position, _rayDirection);

                var area = MeshGroupArea(group);
                Side offset;
                Line2f intersection;
                if (!LiangBarskyClipper.Compute(area, ray, out intersection, out offset))
                    return Vector3f.NaN;

                var foundResult = RaycastGroup(intersection, group);
                if (!foundResult.AnyNan())
                    return foundResult;

                if (offset.HasFlag(Side.X0))
                {
                    group += X0Offset;
                }

                if (offset.HasFlag(Side.X1))
                {
                    group += X1Offset;
                }

                if (offset.HasFlag(Side.Y0))
                {
                    group += Y0Offset;
                }

                if (offset.HasFlag(Side.Y1))
                {
                    group += Y1Offset;
                }

                position = intersection.To;
            }

#if UNITY_EDITOR
            if (counter <= 0)
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

        protected abstract Vector3f RaycastGroup(Line2f line, Vector2i group);
    }
}