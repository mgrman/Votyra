using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Votyra.Core.InputHandling;

namespace Votyra.Core.Editor
{
    [InitializeOnLoad]
    public class UpdateInputActionsEnum : IPreprocessBuildWithReport
    {
        static UpdateInputActionsEnum()
        {
            EditorApplication.playModeStateChanged += stateChange =>
            {
                if (stateChange == PlayModeStateChange.ExitingEditMode)
                    try
                    {
                        UpdateEnum();
                    }
                    catch
                    {
                        EditorApplication.isPlaying = false;
                    }
            };
        }

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            UpdateEnum();
        }

        [MenuItem("Build/Votyra/" + nameof(UpdateInputActionsEnum))]
        public static void UpdateEnum()
        {
            var inputManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];

            var obj = new SerializedObject(inputManager);

            var axisArray = obj.FindProperty("m_Axes");

            if (axisArray.arraySize == 0)
                Debug.Log("No Axes");

            var names = new HashSet<string>();
            for (var i = 0; i < axisArray.arraySize; ++i)
            {
                var axis = axisArray.GetArrayElementAtIndex(i);

                var name = axis.FindPropertyRelative("m_Name")
                    .stringValue;

                names.Add(name);
            }

            var localPath = typeof(InputActions).FullName.Replace('.', Path.DirectorySeparatorChar) + ".cs";

            var enumFilePath = Directory.EnumerateFiles(Application.dataPath, localPath, SearchOption.AllDirectories)
                .FirstOrDefault();

            if (enumFilePath == null)
            {
                Debug.LogWarning($"{nameof(InputActions)} definition in {localPath} cannot be found!");
                return;
            }

            var enumFile = File.ReadAllText(enumFilePath);

            var match = Regex.Match(enumFile, "public enum InputActions[ \r\n\t]*{(.*?)}", RegexOptions.Singleline);

            if (!match.Success)
            {
                Debug.LogWarning($"{nameof(InputActions)} definition in {localPath} cannot be verified!");
                return;
            }

            var matchedOldValues = match.Groups[1]
                .Value;
            var oldValues = Regex.Matches(matchedOldValues, "^[ \t]*([a-zA-Z0-9_]+)[ \t]*,?[ \t]*", RegexOptions.Multiline)
                .Cast<Match>()
                .Select(o => o.Groups[1]
                    .Value.Trim());

            names.ExceptWith(oldValues);

            if (names.Count > 0)
                Debug.LogWarning($"{nameof(InputActions)} definition in {localPath} does not match all axes defined as Inputs! Missing {names}!");
        }
    }
}