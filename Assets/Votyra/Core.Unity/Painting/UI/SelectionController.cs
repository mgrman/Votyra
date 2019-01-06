using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Votyra.Core.Models;
using Zenject;

namespace Votyra.Core.Painting.UI
{
    public class SelectionController : MonoBehaviour, IPointerClickHandler
    {
        [Inject]
        protected DiContainer _diContainer;

        [Inject]
        protected IInstantiator _instantiator;

        public string PropertyName;
        public string TypeName;

        public object Context =>
            gameObject.GetComponentInParent<Context>()
                ?.Value;

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
        }

        // Start is called before the first frame update
        private void Start()
        {
            GetComponentInChildren<Text>()
                .text = Context.GetType()
                .Name;

            var type = Type.GetType(TypeName, false);
            var propertyInfo = type.GetProperty(PropertyName);
            var model = _diContainer.Resolve(type);
            var observableProperty = propertyInfo.GetValue(model) as IObservable<object>;
            observableProperty.Subscribe(o =>
            {
                GetComponent<Toggle>()
                    .isOn = Context == o;
            });

            GetComponent<Toggle>()
                .OnValueChangedAsObservable()
                .MakeLogExceptions()
                .Subscribe(o =>
                {
                    var value = observableProperty.GetType()
                        .GetProperty("Value")
                        .GetValue(observableProperty);
                    if (o && value != Context)
                        observableProperty.GetType()
                            .GetMethod("OnNext")
                            .Invoke(observableProperty, new[] {Context});
                    if (!o && value == Context)
                        observableProperty.GetType()
                            .GetMethod("OnNext")
                            .Invoke(observableProperty, new[] {(object) null});
                });
        }
    }
}