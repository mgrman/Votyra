using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;
using Votyra.Core.Painting.Commands;
using System;
using UniRx;
using Votyra.Core.Models;

namespace Votyra.Core.Painting.UI
{
    public class SelectionController : MonoBehaviour, IPointerClickHandler
    {
        public string TypeName;

        public string PropertyName;

        public object Context => gameObject.GetComponentInParent<Context>()?.Value;

        [Inject]
        protected IInstantiator _instantiator;

        [Inject]
        protected DiContainer _diContainer;

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);


        }

        // Start is called before the first frame update
        private void Start()
        {
            GetComponentInChildren<Text>().text = Context.GetType().Name;

            var type = System.Type.GetType(TypeName, false);
            var propertyInfo = type.GetProperty(PropertyName);
            var model = _diContainer.Resolve(type);
            var observableProperty = propertyInfo.GetValue(model) as IObservable<object>;
            observableProperty.Subscribe(o =>
            {
                this.GetComponent<Toggle>().isOn = Context == o;
            });

            this.GetComponent<Toggle>()
                .OnValueChangedAsObservable()
                .MakeLogExceptions()
                .Subscribe(o =>
                {
                    var value = observableProperty.GetType().GetProperty("Value").GetValue(observableProperty);
                    if (o && value != Context)
                    {
                        observableProperty.GetType().GetMethod("OnNext").Invoke(observableProperty, new[] { Context });
                    }
                    if (!o && value == Context)
                    {
                        observableProperty.GetType().GetMethod("OnNext").Invoke(observableProperty, new[] { (object)null });
                    }
                });
        }
    }
}