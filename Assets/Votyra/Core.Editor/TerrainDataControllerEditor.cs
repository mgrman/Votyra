//c# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;
using Votyra.Core.Unity;
using System.Linq;
using Zenject;
using System.Reflection;
using System.Collections.Generic;
using System;
using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.Editor
{
    [CustomEditor(typeof(TerrainDataController))]
    public class TerrainDataControllerEditor : UnityEditor.Editor
    {
        public static IEnumerable<Type> GetConfigTypes(GameObject algorithPrefab)
        {
            var installers = algorithPrefab.GetComponentInChildren<GameObjectContext>().Installers;

            var container = new DiContainer(false);
            foreach (var installer in installers)
            {
                var containerField = typeof(MonoInstallerBase).GetField("_container", BindingFlags.GetField | BindingFlags.SetField | BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                var tempInstallerGo = new GameObject().AddComponent(installer.GetType());
                try
                {
                    var tempInstaller = tempInstallerGo.GetComponentInChildren<MonoInstallerBase>(true);
                    containerField.SetValue(tempInstaller, container);
                    try
                    {
                        tempInstaller.InstallBindings();
                    }
                    catch { }
                }
                finally
                {
                    GameObject.DestroyImmediate(tempInstallerGo.gameObject);
                }
            }
            return container.AllContracts
                .Select(o => o.Type)
                .Where(o => typeof(IConfig).IsAssignableFrom(o) && !o.IsInterface)
                .ToArray();
        }

        public override void OnInspectorGUI()
        {
            var controller = this.target as TerrainDataController;
            this.DrawDefaultInspector();

            var oldConfigValues = controller.Config;
            var newConfigValues = new List<ConfigItem>();

            var activeAlgorith = controller._availableTerrainAlgorithms[controller._activeTerrainAlgorithm];
            if (activeAlgorith != null)
            {
                var configTypes = GetConfigTypes(activeAlgorith);
                foreach (var configType in configTypes)
                {
                    EditorGUILayout.LabelField($"{configType.Name}");

                    var ctors = configType.GetConstructors();

                    var configItems = (ctors.Length == 1 ? ctors : ctors.Where(o => o.GetCustomAttribute<InjectAttribute>() != null))
                        .SelectMany(o => o.GetParameters()
                            .Select(p => new ConfigItem(p.GetCustomAttribute<InjectAttribute>()?.Id as string, p.ParameterType, null))
                            .Where(a => a.Id != null));

                    foreach (var configItem in configItems)
                    {
                        EditorGUILayout.LabelField($"- {configItem.Id}[{configItem.Type.Name}]");
                        var oldConfigItem = oldConfigValues?.FirstOrDefault(o => o.Id == configItem.Id && o.Type == configItem.Type);

                        object oldValue = oldConfigItem?.Value;
                        object newValue;

                        if (typeof(UnityEngine.Object).IsAssignableFrom(configItem.Type))
                        {
                            newValue = EditorGUILayout.ObjectField(oldValue as UnityEngine.Object, configItem.Type, true);
                        }
                        else if (typeof(bool).IsAssignableFrom(configItem.Type))
                        {
                            var oldBoolValue = oldValue as bool? ?? false;
                            newValue = EditorGUILayout.Toggle(oldBoolValue);
                        }
                        else if (typeof(Vector3i).IsAssignableFrom(configItem.Type))
                        {
                            var oldVector3iValue = oldValue as Vector3i? ?? Vector3i.Zero;
                            var newVector3Value = EditorGUILayout.Vector3Field("", oldVector3iValue.ToVector3f().ToVector3());
                            newValue = newVector3Value.ToVector3f()
                                .RoundToVector3i();
                        }
                        else if (typeof(Vector3f).IsAssignableFrom(configItem.Type))
                        {
                            var oldVector3fValue = oldValue as Vector3f? ?? Vector3f.Zero;
                            newValue = EditorGUILayout.Vector3Field("", oldVector3fValue.ToVector3()).ToVector3f();
                        }
                        else
                        {
                            var oldValueJson = Newtonsoft.Json.JsonConvert.SerializeObject(oldValue);
                            var newJsonValue = EditorGUILayout.TextArea(oldValueJson);
                            try
                            {
                                newValue = Newtonsoft.Json.JsonConvert.DeserializeObject(newJsonValue, configItem.Type);
                            }
                            catch
                            {
                                newValue = null;
                            }
                            newConfigValues.Add(new ConfigItem(configItem.Id, configItem.Type, newValue));
                        }
                        newConfigValues.Add(new ConfigItem(configItem.Id, configItem.Type, newValue));
                    }
                }
            }
            controller.Config = newConfigValues.ToArray();
            if (Application.isPlaying)
            {
                controller.SendMessage("OnValidate", null, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}