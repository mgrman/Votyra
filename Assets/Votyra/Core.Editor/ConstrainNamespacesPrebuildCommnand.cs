using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

[InitializeOnLoad]
public class ConstrainNamespacesPrebuildCommnand : IPreprocessBuild
{
    private const string UnityEngineNamespace = "UnityEngine";
    private const string UnityEngineNamespaceUsage1 = "using " + UnityEngineNamespace;
    private const string UnityEngineNamespaceUsage2 = UnityEngineNamespace + ".";

    public int callbackOrder => int.MaxValue;

    static ConstrainNamespacesPrebuildCommnand()
    {
        EditorApplication.playModeStateChanged += (stateChange) =>
        {
            if (stateChange == PlayModeStateChange.ExitingEditMode)
            {
                try
                {
                    ConstrainNamespaces();
                }
                catch
                {
                    EditorApplication.isPlaying = false;
                }
            }
        };
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        ConstrainNamespaces();
    }

    public static void ConstrainNamespaces()
    {
        var rootPath = Application.dataPath;
        var folderPath = Path.Combine(rootPath, "Votyra");
        var assemblyDefinitionPaths = Directory.EnumerateFiles(folderPath, "*.asmdef", SearchOption.AllDirectories);
        foreach (var assemblyDefinitionPath in assemblyDefinitionPaths)
        {
            var assemblyDefinitionFolder = Path.GetDirectoryName(assemblyDefinitionPath);
            var assemblyName = Path.GetFileNameWithoutExtension(assemblyDefinitionPath);

            if (assemblyName.EndsWith(".Editor") || assemblyName.EndsWith(".Unity"))
            {
                continue;
            }

            var csFiles = Directory.EnumerateFiles(assemblyDefinitionFolder, "*.cs", SearchOption.AllDirectories);
            int errorCount = 0;
            foreach (var csFile in csFiles)
            {
                var csFileLines = File.ReadAllLines(csFile);
                for (int lineIndex = 0; lineIndex < csFileLines.Length; lineIndex++)
                {
                    var line = csFileLines[lineIndex];
                    if (line.Contains(UnityEngineNamespaceUsage1) || line.Contains(UnityEngineNamespaceUsage2))
                    {
                        var relativePath = csFile.StartsWith(rootPath) ? csFile.Substring(rootPath.Length) : csFile;
                        errorCount++;
                        Debug.LogError($"File {relativePath} at line {lineIndex} uses not allowed namespace!");
                    }
                }
            }
            if (errorCount > 0)
            {
                throw new System.Exception($"Some files use code from not allowed namespace:{UnityEngineNamespace}!");
            }
        }
    }
}
