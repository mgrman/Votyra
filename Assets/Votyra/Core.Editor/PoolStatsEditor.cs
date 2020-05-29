using UnityEditor;
using Votyra.Core.Pooling;
using Votyra.Core.Utils;

namespace Votyra.Core
{
    [CustomEditor(typeof(PoolStats)),]
    public class PoolStatsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var behaviour = this.target as PoolStats;
            if (behaviour == null)
            {
                return;
            }

            var pools = behaviour.Pools;

            foreach (var pool in pools)
            {
                EditorGUILayout.LabelField(pool.GetTypeDisplayName());
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(nameof(IPool.PoolCount));
                EditorGUILayout.IntField(pool.PoolCount);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(nameof(IPool.ActiveCount));
                EditorGUILayout.IntField(pool.ActiveCount);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Sum");
                EditorGUILayout.IntField(pool.ActiveCount + pool.PoolCount);
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.Separator();
            }
        }

        public override bool RequiresConstantRepaint() => true;
    }
}
