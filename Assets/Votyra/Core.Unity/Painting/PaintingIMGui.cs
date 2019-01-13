using UnityEngine;
using Votyra.Core.Painting;
using Zenject;

namespace Votyra.Core.Unity.Painting
{
    public class PaintingIMGui:MonoBehaviour
    {
        [Inject]
        private IPaintingModel _paintingModel;
        
       protected void OnGUI()
        {
            foreach (var cmd in _paintingModel.PaintCommands.Value)
            {
                var label = cmd.GetType()
                    .Name;
                if (!GUILayout.Button(label))
                    continue;
                
                var isSelected = _paintingModel.SelectedPaintCommand.Value == cmd;
                _paintingModel.SelectedPaintCommand.OnNext( isSelected ? null : cmd);
            }
            
        }
    }
}