using UnityEngine;
using UnityEngine.EventSystems;
using Votyra.Core.Painting;
using Votyra.Core.Painting.Commands;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Unity.Painting
{
    public class PaintingGui : MonoBehaviour
    {
        [Inject]
        protected IPaintingModel PaintingModel;

        protected void OnGUI()
        {
            GUILayout.BeginArea(new Rect((Screen.width ) - 200, 0, 200, Screen.height));

            IPaintCommand newSelection = PaintingModel.SelectedPaintCommand;
            foreach (var cmd in PaintingModel.PaintCommands)
            {
                var label = cmd.GetTypeDisplayName();
                var wasSelected = newSelection == cmd;
                var isSelected = GUILayout.Toggle(wasSelected, label, GUILayout.Width(200));
                if (wasSelected && !isSelected)
                {
                    newSelection = null;
                }
                else if (!wasSelected && isSelected)
                {
                    newSelection = cmd;
                }
            }
            PaintingModel.SelectedPaintCommand = newSelection;

            GUILayout.Label("Modifiers:",GUILayout.Width(200));
            PaintingModel.IsExtendedModifierActive = GUILayout.Toggle(PaintingModel.IsExtendedModifierActive, "Extend cmd", GUILayout.Width(200));
            PaintingModel.IsInvertModifierActive = GUILayout.Toggle(PaintingModel.IsInvertModifierActive, "Invert cmd", GUILayout.Width(200));

            GUILayout.EndArea();
        }
    }
}