using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Scriban;
using Scriban.Parsing;
using Scriban.Syntax;
using UnityEditor;
using UnityEngine;

namespace Votyra.Core.Editor
{
    [InitializeOnLoad]
    public static class ScribanTemplating
    {
#if UNITY_EDITOR_WIN
        private static string AssetsFolderPath => Application.dataPath.Replace('/', '\\');
#else
        private static string AssetsFolderPath => Application.dataPath;
#endif

        private const uint MaxDimCount = 6;
        private static readonly IReadOnlyList<string> KnownDimensionName = new[] {"X", "Y", "Z", "W"};

        static ScribanTemplating()
        {
            var watcher = new FileSystemWatcher(AssetsFolderPath, "*.scriban-txt");
            watcher.IncludeSubdirectories = true;

            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.Size | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcher.Error += (o, e) => Debug.LogException(e.GetException());

            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.EnableRaisingEvents = true;
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            ExpandTemplate(e.FullPath);
        }

        [MenuItem("Build/Votyra/Expand Scriban Templates")]
        public static void ExpandScribanTemplates()
        {
            var scribanFiles = Directory.EnumerateFiles(AssetsFolderPath, "*.scriban-txt", SearchOption.AllDirectories);
            foreach (var scribanFilePath in scribanFiles)
            {
                ExpandTemplate(scribanFilePath);
            }
        }

        private static void ExpandTemplate(string scribanFilePath)
        {
            try
            {
                var scribanFile = File.ReadAllText(scribanFilePath);
                var directoyPath = Path.GetDirectoryName(scribanFilePath);
                var fileName = Path.GetFileNameWithoutExtension(scribanFilePath);
                var template = Template.Parse(scribanFile);
                if (template.HasErrors)
                {
                    LogMessages(template);
                    return;
                }

                var fileNameTemplate = Template.Parse(fileName);
                if (fileNameTemplate.HasErrors)
                {
                    LogMessages(fileNameTemplate);
                    return;
                }

                for (var dimCount = 1; dimCount <= MaxDimCount; dimCount++)
                {
                    try
                    {
                        var data = new
                        {
                            max_dim = MaxDimCount,
                            dims = dimCount <= KnownDimensionName.Count
                                ? KnownDimensionName.Take(dimCount)
                                    .ToArray()
                                : Enumerable.Range(0, dimCount)
                                    .Select(o => $"X{o}")
                                    .ToArray()
                        };
                        var expandedFileName = fileNameTemplate.Render(data);


                        var resultPath = Path.Combine(directoyPath, expandedFileName);
                        var result = template.Render(data);
                        File.WriteAllText(resultPath, result);
                    }
                    catch (ScriptRuntimeException ex)
                    {
                        Debug.LogError($"Problem with template:{scribanFilePath} in dim:{dimCount}!\n{ex.Span}\n{ex.Message}");
                        Debug.LogException(ex);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Problem with template:{scribanFilePath} in dim:{dimCount}!\n{ex.Message}");
                        Debug.LogException(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Problem with template:{scribanFilePath}!");
                Debug.LogException(ex);
            }
        }

        private static void LogMessages(Template template)
        {
            foreach (var message in template.Messages)
            {
                switch (message.Type)
                {
                    case ParserMessageType.Error:
                        Debug.LogError(message.Message);
                        break;
                    case ParserMessageType.Warning:
                        Debug.LogWarning(message.Message);
                        break;
                    default:
                        Debug.Log(message.Message);
                        break;
                }
            }
        }
    }
}