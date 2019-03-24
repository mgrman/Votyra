using UnityEditor;
using UnityEngine;
using Votyra.Core.Unity.Painting;
using Votyra.Core.Utils;

namespace Votyra.Core.Editor
{
    [CustomEditor(typeof(PaintingGui))]
    public class PaintingGuiEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var controller = target as PaintingGui;
            var _paintingModel = controller.PaintingModel;

            var newSelection = _paintingModel.SelectedPaintCommand;
            foreach (var cmd in _paintingModel.PaintCommands)
            {
                var label = cmd.GetTypeDisplayName();
                var wasSelected = newSelection == cmd;
                var isSelected = EditorGUILayout.Toggle(label, wasSelected);
                if (wasSelected && !isSelected)
                    newSelection = null;
                else if (!wasSelected && isSelected)
                    newSelection = cmd;
            }

            _paintingModel.SelectedPaintCommand = newSelection;

            EditorGUILayout.LabelField("Modifiers:");
            _paintingModel.IsExtendedModifierActive = EditorGUILayout.Toggle("Extend cmd", _paintingModel.IsExtendedModifierActive);
            _paintingModel.IsInvertModifierActive = EditorGUILayout.Toggle("Invert cmd", _paintingModel.IsInvertModifierActive);
        }
    }
}