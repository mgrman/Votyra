using UnityEditor;
using Votyra.Core.Profiling;

namespace Votyra.Core.Editor.Profiling
{
    public class UnityProfilerWindow : EditorWindow
    {
        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/Votyra - AsyncProfiler")]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = (UnityProfilerWindow) GetWindow(typeof(UnityProfilerWindow));
            window.Show();
        }

        private void OnGUI()
        {
            var values = UnityProfilerAggregator.ValuesClone();
            foreach (var value in values)
            {
                EditorGUILayout.LabelField($"{value.Key.Item1} {value.Key.Item2} = {value.Value:#.000} ms");
            }
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}