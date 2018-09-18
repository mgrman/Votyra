using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;
using Votyra.Core.Painting.Commands;

namespace Votyra.Core.Painting.UI
{
    public class PaintCommandButton : MonoBehaviour, IPointerClickHandler
    {
        [Inject]
        private IPaintingModel _paintingModel;

        public IPaintCommand PaintCommand { get; set; }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
            _paintingModel.SelectedPaintCommand.OnNext(PaintCommand);
        }

        // Start is called before the first frame update
        private void Start()
        {
            GetComponentInChildren<Text>().text = PaintCommand.GetType().Name;
        }
    }
}