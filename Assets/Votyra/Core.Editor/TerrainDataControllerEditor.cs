using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Unity;
using Votyra.Core.Utils;
using Object = UnityEngine.Object;

namespace Votyra.Core.Editor
{
    [CustomEditor(typeof(TerrainDataController))]
    public class TerrainDataControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var controller = target as TerrainDataController;
            var list = new ReorderableList(serializedObject, serializedObject.FindProperty(nameof(TerrainDataController._availableTerrainAlgorithms)), true, true, true, true);
            list.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Terrain algorithms");
            };
            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                var isSelected = index == controller._activeTerrainAlgorithm;

                isSelected = EditorGUI.Toggle(new Rect(rect.x, rect.y, 20, EditorGUIUtility.singleLineHeight), GUIContent.none, isSelected, EditorStyles.radioButton);
                if (isSelected)
                    controller._activeTerrainAlgorithm = index;

                EditorGUI.PropertyField(new Rect(rect.x + 20, rect.y, rect.width - 20, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
            };
            serializedObject.Update();
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            Undo.RecordObject(controller, "Test");
            var oldConfigValues = controller.Config.ToList();
            var newConfigValues = new List<ConfigItem>();

            if (controller._activeTerrainAlgorithm < 0 || controller._activeTerrainAlgorithm >= controller._availableTerrainAlgorithms.Length)
                controller._activeTerrainAlgorithm = 0;

            var activeAlgorith = controller._availableTerrainAlgorithms[controller._activeTerrainAlgorithm];
            if (activeAlgorith != null)
            {
                var configTypes = TerrainDataControllerGui.GetConfigTypes(activeAlgorith);
                foreach (var configType in configTypes)
                {
                    if (configType == null)
                        continue;
                    var ctors = configType.GetConstructors();

                    var configItems = (ctors.Length == 1 ? ctors : ctors.Where(o => o.GetCustomAttribute<ConfigInjectAttribute>() != null)).SelectMany(o => o.GetParameters()
                        .Select(p => new ConfigItem(p.GetCustomAttribute<ConfigInjectAttribute>()
                                ?.Id as string,
                            p.ParameterType,
                            null))
                        .Where(a => a.Id != null));

                    if (!configItems.Any())
                        continue;

                    EditorGUILayout.LabelField($"{configType.Name}", EditorStyles.boldLabel);
                    foreach (var configItem in configItems)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"{configItem.Id}[{configItem.Type.Name}]", GUILayout.MinWidth(150));
                        var oldConfigItem = oldConfigValues?.FirstOrDefault(o => o.Id == configItem.Id && o.Type == configItem.Type);

                        if (oldConfigItem != null)
                            oldConfigValues.Remove(oldConfigItem);
                        var oldValue = oldConfigItem?.Value;
                        var newValue = GetNewValue(configItem.Type, oldValue);

                        newConfigValues.Add(new ConfigItem(configItem.Id, configItem.Type, newValue));
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            if (oldConfigValues.Any())
            {
                EditorGUILayout.LabelField("Unused", EditorStyles.boldLabel);
                foreach (var configItem in oldConfigValues.ToArray())
                {
                    EditorGUILayout.BeginHorizontal();
                    var delete = GUILayout.Button("✘", GUILayout.Width(20));
                    EditorGUILayout.LabelField($"{configItem.Id}[{configItem.Type.Name}]", GUILayout.MinWidth(150));
                    var oldConfigItem = oldConfigValues?.FirstOrDefault(o => o.Id == configItem.Id && o.Type == configItem.Type);

                    if (oldConfigItem != null)
                        oldConfigValues.Remove(oldConfigItem);
                    var oldValue = oldConfigItem?.Value;
                    var newValue = GetNewValue(configItem.Type, oldValue);

                    if (!delete)
                        newConfigValues.Add(new ConfigItem(configItem.Id, configItem.Type, newValue));
                    EditorGUILayout.EndHorizontal();
                }
            }

            controller.Config = newConfigValues.ToArray();
            if (Application.isPlaying)
                controller.SendMessage("OnValidate", null, SendMessageOptions.DontRequireReceiver);
            EditorUtility.SetDirty(controller);
        }

        private object GetNewValue(Type type, object oldValue)
        {
            object newValue;

            if (typeof(Object).IsAssignableFrom(type))
            {
                newValue = EditorGUILayout.ObjectField(oldValue as Object, type, true, GUILayout.MaxWidth(200));
            }
            // else if (type.GetCustomAttribute<SerializableAttribute>()!=null)
            // {
            //     newValue = EditorGUILayout.(oldValue , type, true, GUILayout.MaxWidth(200));
            // }
            else if (typeof(bool).IsAssignableFrom(type))
            {
                var oldBoolValue = oldValue as bool? ?? false;
                newValue = EditorGUILayout.Toggle(oldBoolValue, GUILayout.MaxWidth(200));
            }
            else if (typeof(int).IsAssignableFrom(type))
            {
                var oldIntValue = oldValue as int? ?? 0;
                newValue = EditorGUILayout.IntField(oldIntValue, GUILayout.MaxWidth(200));
            }
            else if (typeof(uint).IsAssignableFrom(type))
            {
                var oldIntValue = oldValue as uint? ?? 0;
                newValue = (uint) EditorGUILayout.IntField((int) oldIntValue, GUILayout.MaxWidth(200));
            }
            else if (typeof(float).IsAssignableFrom(type))
            {
                var oldFloatValue = oldValue as float? ?? 0;
                newValue = EditorGUILayout.FloatField(oldFloatValue, GUILayout.MaxWidth(200));
            }
            else if (type.IsEnum)
            {
                var oldEnumValue = oldValue != null && Enum.IsDefined(type, oldValue)
                    ? oldValue as Enum
                    : Enum.GetValues(type)
                        .GetValue(0) as Enum;
                newValue = EditorGUILayout.EnumPopup(oldEnumValue, GUILayout.MaxWidth(200));
            }
            else if (typeof(Vector3i).IsAssignableFrom(type))
            {
                var oldVector3iValue = oldValue as Vector3i? ?? Vector3i.Zero;
                var newVector3Value = EditorGUILayout.Vector3Field("",
                    oldVector3iValue.ToVector3f()
                        .ToVector3(),
                    GUILayout.MaxWidth(200));
                newValue = newVector3Value.ToVector3f()
                    .RoundToVector3i();
            }
            else if (typeof(Vector3f).IsAssignableFrom(type))
            {
                var oldVector3fValue = oldValue as Vector3f? ?? Vector3f.Zero;
                newValue = EditorGUILayout.Vector3Field("", oldVector3fValue.ToVector3(), GUILayout.MaxWidth(200))
                    .ToVector3f();
            }
            else if (typeof(Vector2i).IsAssignableFrom(type))
            {
                var oldVector2iValue = oldValue as Vector2i? ?? Vector2i.Zero;
                var newVector2Value = EditorGUILayout.Vector2Field("",
                    oldVector2iValue.ToVector2f()
                        .ToVector2(),
                    GUILayout.MaxWidth(200));
                newValue = newVector2Value.ToVector2f()
                    .RoundToVector2i();
            }
            else if (typeof(Vector2f).IsAssignableFrom(type))
            {
                var oldVector2fValue = oldValue as Vector2f? ?? Vector2f.Zero;

                newValue = EditorGUILayout.Vector2Field("", oldVector2fValue.ToVector2(), GUILayout.MaxWidth(200))
                    .ToVector2f();
            }
            else if (typeof(AnimationCurve).IsAssignableFrom(type))
            {
                var oldAnimationCurveValue = oldValue as AnimationCurve ?? new AnimationCurve();

                newValue = EditorGUILayout.CurveField("", oldAnimationCurveValue, GUILayout.MaxWidth(200));
            }
            else if (typeof(Area1f).IsAssignableFrom(type))
            {
                var oldVector2fValue = oldValue as Area1f? ?? Area1f.Zero;

                var res = EditorGUILayout.Vector2Field("", new Vector2(oldVector2fValue.Min, oldVector2fValue.Max), GUILayout.MaxWidth(200));

                newValue = Area1f.FromMinAndMax(res.x, res.y);
            }
            else if (type.IsArray)
            {
                var elementType = type.GetElementType();

                EditorGUILayout.BeginVertical();

                var add = GUILayout.Button("Add");

                var newList = Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType)) as IList;

                if (oldValue != null)
                {
                    foreach (var item in (oldValue as IEnumerable))
                    {
                        if (!GUILayout.Button("Remove"))
                        {
                            var newItemValue = GetNewValue(elementType, item);
                            newList.Add(newItemValue);
                        }
                    }
                }

                if (add)
                {
                    newList.Add(Activator.CreateInstance(elementType));
                }

                EditorGUILayout.EndVertical();

                var method = typeof(Enumerable).GetMethod("ToArray", BindingFlags.Public | BindingFlags.Static);
                method = method.MakeGenericMethod(elementType);

                newValue = method.Invoke(null, new[] {newList});
            }
            else
            {
                var props = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

                foreach (var prop in props)
                {
                    EditorGUILayout.LabelField($"{prop.Name}[{prop.FieldType.Name}]", GUILayout.MinWidth(150));
                    prop.SetValue(oldValue, GetNewValue(prop.FieldType, prop.GetValue(oldValue)));
                }

                newValue = oldValue;
                // var oldValueJson = JsonConvert.SerializeObject(oldValue);
                // var newJsonValue = EditorGUILayout.TextArea(oldValueJson, GUILayout.MaxWidth(200));
                // try
                // {
                //     newValue = JsonConvert.DeserializeObject(newJsonValue, type);
                // }
                // catch
                // {
                //     newValue = null;
                // }
            }

            return newValue;
        }
    }
}