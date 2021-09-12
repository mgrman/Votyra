using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Unity;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Editor
{
    public class TerrainDataControllerGui : MonoBehaviour
    {
        private Dictionary<GameObject, IEnumerable<ConfigItem>> _cachedConfigs;
        
        private TerrainDataController _controller;
        
        public static IEnumerable<Type> GetConfigTypes(GameObject algorithPrefab)
        {
            var installers = algorithPrefab.GetComponentInChildren<GameObjectContext>()
                .Installers;

            var container = new DiContainer(false);
            var types = new List<Type>();
            Action<IBindingFinalizer> handler = finalizer =>
            {
                if (finalizer is BindFinalizerWrapper)
                {
                    var bindFinalizer = finalizer as BindFinalizerWrapper;
                    if (bindFinalizer.SubFinalizer is ProviderBindingFinalizer)
                    {
                        var providerBindingFinalizer = bindFinalizer.SubFinalizer as ProviderBindingFinalizer;
                        if (providerBindingFinalizer != null)
                        {
                            types.AddRange(providerBindingFinalizer.BindInfo.ToTypes);
                            return;
                        }
                    }

                    Debug.Log("bindFinalizer.SubFinalizer:" + bindFinalizer.SubFinalizer?.GetType()
                        .FullName);
                }

                Debug.Log("finalizer:" + finalizer.GetType()
                    .FullName);
            };
            container.FinalizeBinding += handler;
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
                    catch
                    {
                    }
                }
                finally
                {
                    DestroyImmediate(tempInstallerGo.gameObject);
                }
            }

            container.FlushBindings();
            container.FinalizeBinding -= handler;
            return types;
        }

        private void Awake()
        {
            _controller = GetComponent<TerrainDataController>();

            _cachedConfigs = new Dictionary<GameObject, IEnumerable<ConfigItem>>();

            foreach (var algorithm in _controller.AvailableTerrainAlgorithms)
            {
                var configTypes = GetConfigTypes(algorithm);
                List<ConfigItem> items = new List<ConfigItem>();
                foreach (var configType in configTypes)
                {
                    var ctors = configType.GetConstructors();
                    var configItems = (ctors.Length == 1 ? ctors : ctors.Where(o => o.GetCustomAttribute<ConfigInjectAttribute>() != null)).SelectMany(o => o.GetParameters()
                        .Select(p => new ConfigItem(p.GetCustomAttribute<ConfigInjectAttribute>()
                            ?.Id as string, p.ParameterType, null))
                        .Where(a => a.Id != null));

                    items.AddRange(configItems);
                }

                _cachedConfigs[algorithm] = items;
            }
        }

        private void OnGUI()
        {
            bool anyChange = false;
            GUILayout.Label("Terrain algorithms:");
            for (var index = 0; index < _controller.AvailableTerrainAlgorithms.Length; index++)
            {
                var terrainAlgorithm = _controller.AvailableTerrainAlgorithms[index];
                var isSelected = index == _controller.ActiveTerrainAlgorithm;
                isSelected = GUILayout.Toggle(isSelected, terrainAlgorithm.name);
                if (isSelected)
                {
                    anyChange = anyChange || _controller.ActiveTerrainAlgorithm != index;
                    _controller.ActiveTerrainAlgorithm = index;
                }
            }



            if (_controller.ActiveTerrainAlgorithm < 0 || _controller.ActiveTerrainAlgorithm >= _controller.AvailableTerrainAlgorithms.Length)
            {
                _controller.ActiveTerrainAlgorithm = 0;
                anyChange = true;
            }

            var activeAlgorith = _controller.AvailableTerrainAlgorithms[_controller.ActiveTerrainAlgorithm];
            if (activeAlgorith != null)
            {
                var newConfigValues = new System.Lazy<List<ConfigItem>>(() => _controller.Config.ToList());
                foreach (var configItem in _cachedConfigs[activeAlgorith])
                {
                    if (!IsSupported(configItem.Type))
                    {
                        continue;
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"{configItem.Id}[{configItem.Type.Name}]", GUILayout.MinWidth(150));
                    var oldConfigItem = _controller.Config?.FirstOrDefault(o => o.Id == configItem.Id && o.Type == configItem.Type);

                    var oldValue = oldConfigItem?.Value;
                    var newValue = GetNewValue(configItem.Type, oldValue);
                    var areEqual = newValue?.Equals(oldValue) ?? oldValue?.Equals(newValue) ?? true;
                    if (!areEqual)
                    {
                        if (oldConfigItem != null)
                            newConfigValues.Value.Remove(oldConfigItem);
                        newConfigValues.Value.Add(new ConfigItem(configItem.Id, configItem.Type, newValue));
                        anyChange = true;
                    }

                    GUILayout.EndHorizontal();
                }

                if (newConfigValues.IsValueCreated)
                {
                    _controller.Config = newConfigValues.Value.ToArray();
                }
            }



            if (anyChange)
            {
                if (Application.isPlaying)
                    _controller.SendMessage("OnValidate", null, SendMessageOptions.DontRequireReceiver);
            }
        }

        private bool IsSupported(Type type)
        {
            if (typeof(bool).IsAssignableFrom(type))
            {
                return true;
            }
            else if (typeof(int).IsAssignableFrom(type))
            {
                return true;
            }
            else if (typeof(float).IsAssignableFrom(type))
            {
                return true;
            }
            else if (type.IsEnum)
            {
                return true;
            }
            else if (typeof(Vector3i).IsAssignableFrom(type))
            {
                return true;
            }
            else if (typeof(Vector3f).IsAssignableFrom(type))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private object GetNewValue(Type type, object oldValue)
        {
            object newValue;

            // if (typeof(Object).IsAssignableFrom(type))
            // {
            //     newValue = EditorGUILayout.ObjectField(oldValue as Object, type, true, GUILayout.MaxWidth(200));
            // }
            // else 
            if (typeof(bool).IsAssignableFrom(type))
            {
                var oldBoolValue = oldValue as bool? ?? false;
                newValue = GUILayout.Toggle(oldBoolValue, "");
            }
            else if (typeof(int).IsAssignableFrom(type))
            {
                var oldIntValue = oldValue as int? ?? 0;
                newValue = Int32.TryParse(GUILayout.TextField(oldIntValue.ToString()), out int intValue) ? intValue : 0;
            }
            else if (typeof(float).IsAssignableFrom(type))
            {
                var oldFloatValue = oldValue as float? ?? 0;
                newValue = float.TryParse(GUILayout.TextField(oldFloatValue.ToString()), out float floatValue) ? floatValue : 0;
            }
            else if (type.IsEnum)
            {
                var oldEnumValue = oldValue != null && Enum.IsDefined(type, oldValue)
                    ? oldValue as Enum
                    : Enum.GetValues(type)
                        .GetValue(0) as Enum;
                newValue = oldEnumValue;

                var options = Enum.GetValues(type);
                foreach (Enum option in options)
                {
                    var selected = GUILayout.Toggle(option.Equals(newValue), Enum.GetName(type, option));
                    if (selected)
                    {
                        newValue = option;
                    }
                }
            }
            else if (typeof(Vector3i).IsAssignableFrom(type))
            {
                var oldVector3IValue = oldValue as Vector3i? ?? Vector3i.Zero;

                var x = Int32.TryParse(GUILayout.TextField(oldVector3IValue.X.ToString()), out int xVal) ? xVal : 0;
                var y = Int32.TryParse(GUILayout.TextField(oldVector3IValue.Y.ToString()), out int yVal) ? yVal : 0;
                var z = Int32.TryParse(GUILayout.TextField(oldVector3IValue.Z.ToString()), out int zVal) ? zVal : 0;

                newValue = new Vector3i(x, y, z);
            }
            else if (typeof(Vector3f).IsAssignableFrom(type))
            {
                var oldVector3FValue = oldValue as Vector3f? ?? Vector3f.Zero;

                var x = float.TryParse(GUILayout.TextField(oldVector3FValue.X.ToString()), out float xVal) ? xVal : 0;
                var y = float.TryParse(GUILayout.TextField(oldVector3FValue.Y.ToString()), out float yVal) ? yVal : 0;
                var z = float.TryParse(GUILayout.TextField(oldVector3FValue.Z.ToString()), out float zVal) ? zVal : 0;

                newValue = new Vector3f(x, y, z);
            }
            else
            {
                newValue = oldValue;
            }

            return newValue;
        }
    }
}