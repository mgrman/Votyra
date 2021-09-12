using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

[InitializeOnLoad]
public class ConstrainNamespacesPrebuildCommnand : IPreprocessBuildWithReport
{
    private const string UnityEngineNamespace = "UnityEngine";
    
    private const string UnityEngineNamespaceUsage1 = "using " + UnityEngineNamespace;
    
    private const string UnityEngineNamespaceUsage2 = UnityEngineNamespace + ".";

    static ConstrainNamespacesPrebuildCommnand()
    {
        EditorApplication.playModeStateChanged += stateChange =>
        {
            if (stateChange == PlayModeStateChange.ExitingEditMode)
                try
                {
                    ConstrainNamespaces();
                }
                catch
                {
                    EditorApplication.isPlaying = false;
                }
        };
    }

    public int callbackOrder => int.MaxValue;

    public void OnPreprocessBuild(BuildReport report)
    {
        ConstrainNamespaces();
    }

    [MenuItem("Build/Votyra/" + nameof(ConstrainNamespaces))]
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
                continue;

            var csFiles = Directory.EnumerateFiles(assemblyDefinitionFolder, "*.cs", SearchOption.AllDirectories);
            var errorCount = 0;
            foreach (var csFile in csFiles)
            {
                var csFileLines = File.ReadAllLines(csFile);
                for (var lineIndex = 0; lineIndex < csFileLines.Length; lineIndex++)
                {
                    var line = csFileLines[lineIndex];
                    if (line.Contains(UnityEngineNamespaceUsage1) || line.Contains(UnityEngineNamespaceUsage2))
                    {
                        var relativePath = csFile.StartsWith(rootPath) ? csFile.Substring(rootPath.Length) : csFile;
                        errorCount++;
                        Debug.LogWarning($"File {relativePath} at line {lineIndex} uses not allowed namespace!");
                    }
                }
            }

            // if (errorCount > 0)
            // {
            //     throw new System.Exception($"Some files use code from not allowed namespace:{UnityEngineNamespace}!");
            // }
        }
    }

    public void OnPreprocessBuild(BuildReport report, string path)
    {
        ConstrainNamespaces();
    }
}