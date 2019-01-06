using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Votyra.Core.Painting.Commands;
using Zenject;

namespace Votyra.Core.Painting.UI
{
    public class PaintCommandButton : MonoBehaviour, IPointerClickHandler
    {
        [Inject]
        protected IPaintingModel _paintingModel;

        public IPaintCommand PaintCommand { get; set; }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
            _paintingModel.SelectedPaintCommand.OnNext(PaintCommand);
        }

        // Start is called before the first frame update
        private void Start()
        {
            GetComponentInChildren<Text>()
                .text = PaintCommand.GetType()
                .Name;
        }
    }
}