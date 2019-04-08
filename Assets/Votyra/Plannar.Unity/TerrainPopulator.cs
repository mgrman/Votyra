using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Votyra.Core;
using Votyra.Core.Models;
using Votyra.Core.Utils;
using Votyra.Core.TerrainMeshes;
using Zenject;
using Random = System.Random;

namespace Votyra.Plannar.Unity
{
    public class TerrainPopulator
    {
        private readonly GameObject _root;
        private Vector2i _cellInGroupCount;

        private Dictionary<Vector2i, IEnumerable<GameObject>> _trees = new Dictionary<Vector2i, IEnumerable<GameObject>>();

        public TerrainPopulator(ITerrainGeneratorManager2i manager, ITerrainConfig config, [Inject(Id = "root")] GameObject root)
        {
            _root = root;
            manager.ChangedTerrain += ChangedTerrain;
            manager.RemovedTerrain += RemovedTerrain;
            _cellInGroupCount = config.CellInGroupCount.XY();
        }

        private void ChangedTerrain(Vector2i group, ITerrainMesh2f terrain)
        {
            foreach (var go in _trees.TryGetValue(group) ?? Enumerable.Empty<GameObject>())
            {
                GameObject.Destroy(go);
            }

            var list = new List<GameObject>();
            var rnd = new Random(group.X + group.Y * _cellInGroupCount.X);
            var count= rnd.Next(0, 10);
            for (int i = 0; i < count; i++)
            {
                var pos = terrain.MeshBounds.Min.XY() + new Vector2f((float) rnd.NextDouble() * _cellInGroupCount.X, (float) rnd.NextDouble() * _cellInGroupCount.Y);
                var value = terrain.Raycast(pos);

                var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.SetParent(_root.transform);
                go.transform.localPosition = new Vector3(pos.X, pos.Y, value + 1.5f);
                var scale = (float) rnd.NextDouble() / 0.4f + 0.8f;
                go.transform.localScale = new Vector3(scale, scale, scale);
                list.Add(go);
            }

            _trees[group] = list;
        }

        private void RemovedTerrain(Vector2i group)
        {
            foreach (var go in _trees.TryGetValue(group) ?? Enumerable.Empty<GameObject>())
            {
                GameObject.Destroy(go);
            }

            _trees.TryRemoveAndReturnValue(group);
        }
    }
}