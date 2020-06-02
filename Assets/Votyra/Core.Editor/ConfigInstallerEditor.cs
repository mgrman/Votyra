using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Votyra.Core.Logging;
using Votyra.Core.Models;
using Votyra.Core.Unity;
using Votyra.Core.Utils;
using Zenject;
using Object = UnityEngine.Object;

namespace Votyra.Core.Editor
{
    [CustomEditor(typeof(ConfigInstaller))]
    public class ConfigInstallerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var controller = this.target as ConfigInstaller;

            Undo.RecordObject(controller, "Test");
            var oldConfigValues = controller.Config?.ToList() ?? new List<ConfigItem>();
            var newConfigValues = new List<ConfigItem>();

            var configTypes = GetConfigTypes(controller.gameObject);

            var isChanged = false;
            foreach (var configType in configTypes)
            {
                if (configType == null)
                {
                    continue;
                }

                var ctors = configType.GetConstructors();

                var configItems = (ctors.Length == 1 ? ctors : ctors.Where(o => o.GetCustomAttribute<ConfigInjectAttribute>() != null)).SelectMany(o => o.GetParameters()
                    .Select(p => CreateConfigItem(p))
                    .Where(a => a.id != null));

                if (!configItems.Any())
                {
                    continue;
                }

                EditorGUILayout.LabelField($"{configType.Name}", EditorStyles.boldLabel);
                foreach (var configItem in configItems)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"{configItem.id}[{configItem.parameterType.Name}]", GUILayout.MinWidth(150));
                    var oldConfigItem = oldConfigValues?.FirstOrDefault(o => (o.Id == configItem.id) && (o.Type == configItem.parameterType));

                    if (oldConfigItem != null)
                    {
                        oldConfigValues.Remove(oldConfigItem);
                    }

                    var oldValue = oldConfigItem?.Value;
                    var configItemChange = this.GetNewValue(configItem.parameterType, oldValue, out var newValue);
                    if (configItemChange)
                    {
                        isChanged = true;
                        newConfigValues.Add(new ConfigItem(configItem.id, configItem.parameterType, newValue));
                    }
                    else if (oldConfigItem != null)
                    {
                        newConfigValues.Add(oldConfigItem);
                    }
                    else
                    {
                        try
                        {
                            object defaultValue = null;
                            if (configItem.parameterType.IsEnum)
                            {
                                defaultValue = Enum.GetValues(configItem.parameterType)
                                    .GetValue(0);
                            }
                            else
                            {
                                var defaultCtor = configItem.parameterType.GetConstructor(Type.EmptyTypes);
                                if (defaultCtor != null)
                                {
                                    defaultValue = defaultCtor.Invoke(null);
                                }
                            }

                            newConfigValues.Add(new ConfigItem(configItem.id, configItem.parameterType, defaultValue));
                        }
                        catch (Exception ex)
                        {
                            StaticLogger.LogException(ex);
                        }

                        isChanged = true;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            if (oldConfigValues.Any())
            {
                EditorGUILayout.LabelField("Unused", EditorStyles.boldLabel);
                foreach (var configItem in oldConfigValues.ToArray())
                {
                    if (configItem.Type == null)
                    {
                        continue;
                    }

                    EditorGUILayout.BeginHorizontal();
                    var delete = GUILayout.Button("✘", GUILayout.Width(20));
                    EditorGUILayout.LabelField($"{configItem.Id}[{configItem.Type.Name}]", GUILayout.MinWidth(150));
                    var oldConfigItem = oldConfigValues?.FirstOrDefault(o => (o.Id == configItem.Id) && (o.Type == configItem.Type));

                    if (oldConfigItem != null)
                    {
                        oldConfigValues.Remove(oldConfigItem);
                    }

                    var oldValue = oldConfigItem?.Value;
                    var configItemChange = this.GetNewValue(configItem.Type, oldValue, out var newValue);
                    if (!delete)
                    {
                        if (configItemChange)
                        {
                            isChanged = true;
                            newConfigValues.Add(new ConfigItem(configItem.Id, configItem.Type, newValue));
                        }
                        else
                        {
                            newConfigValues.Add(configItem);
                        }
                    }
                    else
                    {
                        isChanged = true;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            if (isChanged)
            {
                controller.Config = newConfigValues.ToArray();
                EditorUtility.SetDirty(controller);
            }
        }

        private static (string id, Type parameterType) CreateConfigItem(ParameterInfo p)
        {
            var idAttr = p.GetCustomAttribute<ConfigInjectAttribute>();
            var id = idAttr?.Id?.ToString();
            return (id, p.ParameterType);
        }

        private static IEnumerable<Type> GetConfigTypes(GameObject algorithmPrefab)
        {
            IEnumerable<IInstaller> installers;

            var autoContext = algorithmPrefab.GetComponent<AutoGameObjectContext>();
            if (autoContext != null)
            {
                installers = autoContext.GetInstallers();
            }
            else
            {
                var context = algorithmPrefab.GetComponent<GameObjectContext>();
                installers = context.Installers;
            }

            var container = new DiContainer(false);
            var tempGameObject = new GameObject();
            try
            {
                foreach (var installer in installers)
                {
                    var tempInstallerGo = tempGameObject.AddComponent(installer.GetType());
                    var tempInstaller = tempInstallerGo.GetComponentInChildren<MonoInstallerBase>(true);

                    try
                    {
                        container.Inject(tempInstaller, new object[0]);
                        tempInstaller.InstallBindings();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        DestroyImmediate(tempInstaller);
                    }
                }

                container.FlushBindings();
            }
            finally
            {
                DestroyImmediate(tempGameObject);
            }

            var types = new List<Type>();
            foreach (var provider in container.AllProviders)
            {
                try
                {
                    var instanceType = provider.GetInstanceType(new InjectContext(container, typeof(object)));
                    types.Add(instanceType);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }

            return types;
        }

        private bool GetNewValue(Type type, object oldValue, out object newValue)
        {
            if (typeof(Object).IsAssignableFrom(type))
            {
                var oldUnityObject = oldValue as Object;
                var newUnityObject = EditorGUILayout.ObjectField(oldUnityObject, type, true, GUILayout.MaxWidth(200));
                newValue = newUnityObject;
                return oldUnityObject != newUnityObject;
            }

            if (typeof(bool).IsAssignableFrom(type))
            {
                var oldBoolValue = oldValue as bool? ?? false;
                var newBoolValue = EditorGUILayout.Toggle(oldBoolValue, GUILayout.MaxWidth(200));
                newValue = newBoolValue;
                return newBoolValue != oldBoolValue;
            }

            if (typeof(int).IsAssignableFrom(type))
            {
                var oldIntValue = oldValue as int? ?? 0;
                var newIntValue = EditorGUILayout.IntField(oldIntValue, GUILayout.MaxWidth(200));
                newValue = newIntValue;
                return newIntValue != oldIntValue;
            }

            if (typeof(uint).IsAssignableFrom(type))
            {
                var oldIntValue = oldValue as uint? ?? 0;
                var newIntValue = (uint)EditorGUILayout.IntField((int)oldIntValue, GUILayout.MaxWidth(200));
                newValue = newIntValue;
                return newIntValue != oldIntValue;
            }

            if (typeof(float).IsAssignableFrom(type))
            {
                var oldFloatValue = oldValue as float? ?? 0;
                var newFloatValue = EditorGUILayout.FloatField(oldFloatValue, GUILayout.MaxWidth(200));
                newValue = newFloatValue;
                return newFloatValue != oldFloatValue;
            }

            if (typeof(string).IsAssignableFrom(type))
            {
                var oldStringValue = oldValue as string;
                var newStringValue = EditorGUILayout.TextField(oldStringValue, GUILayout.MaxWidth(200));
                newValue = newStringValue;
                return newStringValue != oldStringValue;
            }

            if (type.IsEnum)
            {
                var oldEnumValue = (oldValue != null) && Enum.IsDefined(type, oldValue)
                    ? oldValue as Enum
                    : Enum.GetValues(type)
                        .GetValue(0) as Enum;

                var newEnumValue = EditorGUILayout.EnumPopup(oldEnumValue, GUILayout.MaxWidth(200));
                newValue = newEnumValue;
                return !newEnumValue.Equals(oldEnumValue);
            }

            if (typeof(Vector3i).IsAssignableFrom(type))
            {
                var oldVector3iValue = oldValue as Vector3i? ?? Vector3i.Zero;
                var oldVector3Value = oldVector3iValue.ToVector3f()
                    .ToVector3();

                var newVector3iValue = EditorGUILayout.Vector3Field(string.Empty, oldVector3Value, GUILayout.MaxWidth(200))
                    .ToVector3F()
                    .RoundToVector3i();

                newValue = newVector3iValue;
                return newVector3iValue != oldVector3iValue;
            }

            if (typeof(Vector3f).IsAssignableFrom(type))
            {
                var oldVector3fValue = oldValue as Vector3f? ?? Vector3f.Zero;
                var newVector3fValue = EditorGUILayout.Vector3Field(string.Empty, oldVector3fValue.ToVector3(), GUILayout.MaxWidth(200))
                    .ToVector3F();

                newValue = newVector3fValue;
                return newVector3fValue != oldVector3fValue;
            }

            if (typeof(Vector2i).IsAssignableFrom(type))
            {
                var oldVector2iValue = oldValue as Vector2i? ?? Vector2i.Zero;
                var oldVector2Value = oldVector2iValue.ToVector2f()
                    .ToVector2();

                var newVector2iValue = EditorGUILayout.Vector2Field(string.Empty, oldVector2Value, GUILayout.MaxWidth(200))
                    .ToVector2f()
                    .RoundToVector2i();

                newValue = newVector2iValue;
                return newVector2iValue != oldVector2iValue;
            }

            if (typeof(Vector2f).IsAssignableFrom(type))
            {
                var oldVector2fValue = oldValue as Vector2f? ?? Vector2f.Zero;
                var newVector2fValue = EditorGUILayout.Vector2Field(string.Empty, oldVector2fValue.ToVector2(), GUILayout.MaxWidth(200))
                    .ToVector2f();

                newValue = newVector2fValue;
                return newVector2fValue != oldVector2fValue;
            }

            if (typeof(AnimationCurve).IsAssignableFrom(type))
            {
                var oldAnimationCurveValue = oldValue as AnimationCurve;
                var newAnimationCurveValue = EditorGUILayout.CurveField(string.Empty, oldAnimationCurveValue ?? new AnimationCurve(), GUILayout.MaxWidth(200));
                newValue = newAnimationCurveValue;
                return !(newAnimationCurveValue?.Equals(oldAnimationCurveValue) ?? (oldAnimationCurveValue == null));
            }

            if (typeof(Area1f).IsAssignableFrom(type))
            {
                var oldArea1fValue = oldValue as Area1f?;

                var tempVector2 = EditorGUILayout.Vector2Field(string.Empty, new Vector2(oldArea1fValue?.Min ?? 0, oldArea1fValue?.Max ?? 0), GUILayout.MaxWidth(200));
                var newArea1fValue = Area1f.FromMinAndMax(tempVector2.x, tempVector2.y);
                newValue = newArea1fValue;
                return (oldArea1fValue == null) || !newArea1fValue.Equals(oldArea1fValue.Value);
            }

            if (type.IsArray)
            {
                var change = false;
                var elementType = type.GetElementType();

                EditorGUILayout.BeginVertical();

                var add = GUILayout.Button("Add");

                var newList = Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType)) as IList;

                if (oldValue != null)
                {
                    foreach (var item in oldValue as IEnumerable)
                    {
                        if (!GUILayout.Button("Remove"))
                        {
                            var itemChange = this.GetNewValue(elementType, item, out var newItemValue);
                            if (itemChange)
                            {
                                change = true;
                                newList.Add(newItemValue);
                            }
                            else
                            {
                                newList.Add(item);
                            }
                        }
                        else
                        {
                            change = true;
                        }
                    }
                }

                if (add)
                {
                    newList.Add(Activator.CreateInstance(elementType));
                    change = true;
                }

                EditorGUILayout.EndVertical();

                var method = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray), BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
                method = method.MakeGenericMethod(elementType);

                var parameters = new[]
                {
                    newList,
                };

                newValue = method.Invoke(null, parameters);

                return change;
            }
            else
            {
                var change = false;
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);

                newValue = Activator.CreateInstance(type);
                foreach (var prop in props)
                {
                    EditorGUILayout.LabelField($"{prop.Name}[{prop.PropertyType.Name}]", GUILayout.MinWidth(150));
                    var oldFieldValue = prop.GetValue(oldValue);
                    var propChange = this.GetNewValue(prop.PropertyType, oldFieldValue, out var newFieldValue);
                    if (propChange)
                    {
                        change = true;
                        prop.SetValue(newValue, newFieldValue);
                    }
                    else
                    {
                        prop.SetValue(newValue, oldFieldValue);
                    }
                }

                return change;
            }
        }
    }
}
