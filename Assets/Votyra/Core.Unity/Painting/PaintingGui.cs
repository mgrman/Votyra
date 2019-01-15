using UnityEngine;
using UnityEngine.EventSystems;
using Votyra.Core.Painting;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core.Unity.Painting
{
    public class PaintingGui : MonoBehaviour
    {
        [Inject]
        protected IPaintingModel _paintingModel;

        protected void OnGUI()
        {
            GUILayout.BeginArea(new Rect((Screen.width ) - 200, 0, 200, Screen.height));

            foreach (var cmd in _paintingModel.PaintCommands)
            {
                var label = cmd.GetTypeDisplayName();
                if (!GUILayout.Button(label,GUILayout.Width(200)))
                    continue;

                var isSelected = _paintingModel.SelectedPaintCommand == cmd;
                _paintingModel.SelectedPaintCommand=isSelected ? null : cmd;
            }

            GUILayout.EndArea();
        }
    }
}