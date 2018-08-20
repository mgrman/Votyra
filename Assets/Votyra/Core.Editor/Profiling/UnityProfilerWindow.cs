using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEditor;
using UnityEngine.Profiling;
using Votyra.Core.Profiling;

namespace Votyra.Core.Editor.Profiling
{
    public class UnityProfilerWindow : EditorWindow
    {
        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/Votyra - AsyncProfiler")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            UnityProfilerWindow window = (UnityProfilerWindow)EditorWindow.GetWindow(typeof(UnityProfilerWindow));
            window.Show();
        }

        void OnGUI()
        {
            var values = UnityProfilerAggregator.ValuesClone();
            foreach (var value in values)
            {
                EditorGUILayout.LabelField($"{value.Key.Item1} {value.Key.Item2} = {value.Value:#.000} ms");
            }
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}