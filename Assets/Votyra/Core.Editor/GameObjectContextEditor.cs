using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Votyra.Core.Unity;

namespace Zenject
{
    [CustomEditor(typeof(AutoGameObjectContext))]
    public class AutoGameObjectContextEditor : Editor
    {
        private ReorderableList installersList;
        private List<MonoInstaller> list;

        private void OnEnable()
        {
            this.list = new List<MonoInstaller>();
            this.installersList = new ReorderableList(this.list, typeof(MonoInstaller), true, true, false, false);
            this.installersList.drawHeaderCallback += rect =>
            {
                GUI.Label(rect, new GUIContent("Autopopulated Mono Installers", "Autopopulated Mono Installers from this GameObject."));
            };

            this.installersList.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.width -= 40;
                rect.x += 20;
                EditorGUI.ObjectField(rect, this.list[index], typeof(MonoInstaller), true);
            };
        }

        public override void OnInspectorGUI()
        {
            var context = this.target as AutoGameObjectContext;

            this.list.Clear();
            this.list.AddRange(context.GetInstallers());

            GUI.enabled = false;
            this.installersList.DoLayoutList();
            EditorGUI.EndDisabledGroup();
            GUI.enabled = true;
        }
    }
}
