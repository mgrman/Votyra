using System.Collections.Generic;
using UnityEngine;
using Votyra.Core;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;
using Votyra.Core.Utils;
using Zenject;
using Random = System.Random;

namespace Votyra.Plannar.Unity
{
    public class TerrainPopulator : MonoBehaviour
    {
        private readonly Dictionary<Vector2i, List<Matrix4x4>> trees = new Dictionary<Vector2i, List<Matrix4x4>>();
        private Vector2i cellInGroupCount;

        private int index;

        private Pool<List<Matrix4x4>> pool;
        private PopulatorConfigItem populatorConfig;
        private GameObject root;

        public void Initialize(IUnityTerrainGeneratorManager2i manager, ITerrainConfig config, PopulatorConfigItem populatorConfig, int index, [Inject(Id = "root"),]
            GameObject root)
        {
            this.root = root;
            this.cellInGroupCount = config.CellInGroupCount.XY();

            this.pool = new Pool<List<Matrix4x4>>(() => new List<Matrix4x4>((int)populatorConfig.CountPerGroup));
            this.populatorConfig = populatorConfig;
            this.index = index;

            manager.NewTerrain += this.NewTerrain;
            manager.ChangedTerrain += this.ChangedTerrain;
            manager.RemovedTerrain += this.RemovedTerrain;
        }

        private void NewTerrain(Vector2i arg1, ITerrainMesh2f arg2)
        {
            this.ChangedTerrain(arg1, arg2);
        }

        private void Update()
        {
            foreach (var groupTrees in this.trees.Values)
            {
                Graphics.DrawMeshInstanced(this.populatorConfig.Mesh, 0, this.populatorConfig.Material, groupTrees);
            }
        }

        private void ChangedTerrain(Vector2i group, ITerrainMesh2f terrain)
        {
            var list = this.trees.TryGetValue(group) ?? this.pool.GetRaw();
            list.Clear();

            int seed;
            unchecked
            {
                seed = (group.X + (group.Y * this.cellInGroupCount.X)) * this.index;
            }

            var rnd = new Random(seed);
            for (var i = 0; i < this.populatorConfig.CountPerGroup; i++)
            {
                var posLocalXy = terrain.MeshBounds.Min.XY() + new Vector2f((float)rnd.NextDouble() * this.cellInGroupCount.X, (float)rnd.NextDouble() * this.cellInGroupCount.Y);

                var value = terrain.Raycast(posLocalXy);

                if (value.AnyNan())
                {
                    continue;
                }

                var heightProb = this.populatorConfig.HeightCurve.Evaluate(value);
                if (rnd.NextDouble() > heightProb)
                {
                    continue;
                }

                var posLocal = new Vector3(posLocalXy.X, posLocalXy.Y, value);
                var posWorld = this.root.transform.TransformPoint(posLocal);

                var scale = ((float)rnd.NextDouble() * this.populatorConfig.UniformScaleVariance.Size) + this.populatorConfig.UniformScaleVariance.Min;
                var rotation = (float)(rnd.NextDouble() * 360);

                var res = Matrix4x4.TRS(posWorld, Quaternion.Euler(0, rotation, 0), new Vector3(scale, scale, scale));
                list.Add(res);
            }

            this.trees[group] = list;
        }

        private void RemovedTerrain(Vector2i group, ITerrainMesh2f mesh)
        {
            var array = this.trees.TryRemoveAndReturnValue(group);
            if (array != null)
            {
                this.pool.ReturnRaw(array);
            }
        }
    }
}
