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

        private Pool<List<Matrix4x4>> _pool;

        private Dictionary<Vector2i, List<Matrix4x4>> _trees = new Dictionary<Vector2i, List<Matrix4x4>>();

        private int _index;
        private PopulatorConfigItem _populatorConfig;
        
        public void Initialize(IUnityTerrainGeneratorManager2i manager, ITerrainConfig config, PopulatorConfigItem populatorConfig, int index, [Inject(Id = "root")] GameObject root)
        {
            
            _root = root;
            _cellInGroupCount = config.CellInGroupCount.XY();

            _pool = new Pool<List<Matrix4x4>>(() => new List<Matrix4x4>((int) populatorConfig.CountPerGroup));
            _populatorConfig = populatorConfig;
            _index = index;
            
            manager.NewTerrain += NewTerrain;
            manager.ChangedTerrain += ChangedTerrain;
            manager.RemovedTerrain += RemovedTerrain;
        }

        private void NewTerrain(Vector2i arg1, ITerrainMesh2f arg2)
        {
            ChangedTerrain(arg1, arg2);
        }

        private void Update()
        {
            foreach (var groupTrees in _trees.Values)
            {
                Graphics.DrawMeshInstanced(_populatorConfig.Mesh, 0, _populatorConfig.Material, groupTrees);
            }
        }

        private void ChangedTerrain(Vector2i group, ITerrainMesh2f terrain)
        {
            var list = _trees.TryGetValue(group) ?? _pool.GetRaw();
            list.Clear();

            int seed;
            unchecked
            {
                seed = (group.X + group.Y * _cellInGroupCount.X) * _index;
            }

            var rnd = new Random(seed);
            for (int i = 0; i < _populatorConfig.CountPerGroup; i++)
            {
                var posLocalXY = terrain.MeshBounds.Min.XY() + new Vector2f((float) rnd.NextDouble() * _cellInGroupCount.X, (float) rnd.NextDouble() * _cellInGroupCount.Y);

                var value = terrain.Raycast(posLocalXY);

                if (value.AnyNan())
                {
                    continue;
                }

                var heightProb = _populatorConfig.HeightCurve.Evaluate(value);
                if (rnd.NextDouble() > heightProb)
                {
                    continue;
                }
                
                var posLocal = new Vector3(posLocalXY.X, posLocalXY.Y, value);
                var posWorld = _root.transform.TransformPoint(posLocal);

                var scale = (float) rnd.NextDouble() * _populatorConfig.UniformScaleVariance.Size + _populatorConfig.UniformScaleVariance.Min;
                var rotation = (float) (rnd.NextDouble() * 360);

                var res = Matrix4x4.TRS(posWorld, Quaternion.Euler(0, rotation, 0), new Vector3(scale, scale, scale));
                list.Add(res);
            }

            _trees[group] = list;
        }

        private void RemovedTerrain(Vector2i group, ITerrainMesh2f mesh)
        {
            var array = _trees.TryRemoveAndReturnValue(group);
            if (array != null)
            {
                _pool.ReturnRaw(array);
            }
        }
    }
}