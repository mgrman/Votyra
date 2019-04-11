using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Votyra.Core;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Utils;
using Votyra.Core.TerrainMeshes;
using Zenject;
using Random = System.Random;

namespace Votyra.Plannar.Unity
{
    public class TerrainPopulator : MonoBehaviour
    {
        private GameObject _root;
        private Vector2i _cellInGroupCount;

        private Pool<Matrix4x4[]> _pool;

        private Dictionary<Vector2i, Matrix4x4[]> _trees = new Dictionary<Vector2i, Matrix4x4[]>();
        
        
        private Mesh _mesh;
        
        private Material _material;

        public void Initialize(ITerrainGeneratorManager2i manager, ITerrainConfig config, PopulatorConfigItem populatorConfig, [Inject(Id = "root")] GameObject root)
        {
            _root = root;
            manager.ChangedTerrain += ChangedTerrain;
            manager.RemovedTerrain += RemovedTerrain;
            _cellInGroupCount = config.CellInGroupCount.XY();

            _pool = new Pool<Matrix4x4[]>(() => new Matrix4x4[populatorConfig.CountPerGroup]);
            _mesh = populatorConfig.Mesh;
            _material = populatorConfig.Material;
        }

        private void Update()
        {
            foreach (var groupTrees in _trees.Values)
            {
                Graphics.DrawMeshInstanced(_mesh, 0, _material, groupTrees, groupTrees.Length);
            }
        }

        private void ChangedTerrain(Vector2i group, ITerrainMesh2f terrain)
        {
            var list = _trees.TryGetValue(group) ?? _pool.GetRaw();
            var rnd = new Random(group.X + group.Y * _cellInGroupCount.X);
            for (int i = 0; i < list.Length; i++)
            {
                var posLocalXY = terrain.MeshBounds.Min.XY() + new Vector2f((float) rnd.NextDouble() * _cellInGroupCount.X, (float) rnd.NextDouble() * _cellInGroupCount.Y);

                var value = terrain.Raycast(posLocalXY);
                var posLocal = new Vector3(posLocalXY.X, posLocalXY.Y, value + 1.5f);
                var posWorld = _root.transform.TransformPoint(posLocal);

                var scale = (float) rnd.NextDouble() / 0.4f + 0.8f;

                list[i] = Matrix4x4.TRS(posWorld, Quaternion.identity, new Vector3(scale, scale, scale));
            }

            _trees[group] = list;
        }

        private void RemovedTerrain(Vector2i group)
        {
            var array = _trees.TryRemoveAndReturnValue(group);
            if (array != null)
            {
                _pool.ReturnRaw(array);
            }
        }
    }
}