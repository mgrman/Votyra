using UnityEngine;
using Votyra.Core.Painting;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Unity.Painting
{
    public class PaintingGui : MonoBehaviour
    {
        [Inject]
        protected IPaintingModel _paintingModel;

#if UNITY_EDITOR
        public bool showPaintingGui;
#endif
//         
//         protected void OnGUI()
//         {
//
// #if UNITY_EDITOR
//             if (!showPaintingGui)
//             {
//                 return;
//             }
// #endif 
//             
//             GUILayout.BeginArea(new Rect(Screen.width - 200, 0, 200, Screen.height));
//
//             var newSelection = _paintingModel.SelectedPaintCommand;
//             foreach (var cmd in _paintingModel.PaintCommands)
//             {
//                 var label = cmd.GetTypeDisplayName();
//                 var wasSelected = newSelection == cmd;
//                 var isSelected = GUILayout.Toggle(wasSelected, label, GUILayout.Width(200));
//                 if (wasSelected && !isSelected)
//                     newSelection = null;
//                 else if (!wasSelected && isSelected)
//                     newSelection = cmd;
//             }
//
//             _paintingModel.SelectedPaintCommand = newSelection;
//
//             GUILayout.Label("Modifiers:", GUILayout.Width(200));
//             _paintingModel.IsExtendedModifierActive = GUILayout.Toggle(_paintingModel.IsExtendedModifierActive, "Extend cmd", GUILayout.Width(200));
//             _paintingModel.IsInvertModifierActive = GUILayout.Toggle(_paintingModel.IsInvertModifierActive, "Invert cmd", GUILayout.Width(200));
//
//             GUILayout.EndArea();
//         }
    }
}