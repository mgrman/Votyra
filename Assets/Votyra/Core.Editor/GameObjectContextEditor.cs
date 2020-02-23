using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Votyra.Core.Unity;

namespace Zenject
{
    [CustomEditor(typeof(AutoGameObjectContext))]
    public class AutoGameObjectContextEditor : UnityEditor.Editor
    {
        private ReorderableList installersList;
        private List<MonoInstaller> list;

        private void OnEnable()
        {
            list = new List<MonoInstaller>();
            installersList = new ReorderableList(list, typeof(MonoInstaller), true, true, false, false);
            installersList.drawHeaderCallback += rect =>
            {
                GUI.Label(rect, new GUIContent("Autopopulated Mono Installers", "Autopopulated Mono Installers from this GameObject."));
            };
            installersList.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.width -= 40;
                rect.x += 20;
                EditorGUI.ObjectField(rect, list[index], typeof(MonoInstaller), true);
            };
        }

        public override void OnInspectorGUI()
        {
            var context = target as AutoGameObjectContext;

            list.Clear();
            list.AddRange(context.GetInstallers());

            GUI.enabled = false;
            installersList.DoLayoutList();
            EditorGUI.EndDisabledGroup();
            GUI.enabled = true;
        }
    }
}