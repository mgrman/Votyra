using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace Votyra.Core.Painting.UI
{
    public class BindToCollectionController : MonoBehaviour
    {
        [Inject]
        protected DiContainer _diContainer;

        [Inject]
        protected IInstantiator _instantiator;

        private IDisposable _subscription;

        public string PropertyName;
        public string TypeName;

        // Start is called before the first frame update
        private void Start()
        {
            var type = Type.GetType(TypeName, false);
            var propertyInfo = type.GetProperty(PropertyName);
            var model = _diContainer.Resolve(type);
            var observableProperty = propertyInfo.GetValue(model) as IObservable<object>;

            _subscription = observableProperty.Subscribe(o =>
            {
                var enumeration = o as IEnumerable<object>;

                var childrenCount = transform.childCount;
                for (var i = childrenCount - 1; i >= 0; i--)
                {
                    var child = transform.GetChild(i)
                        .gameObject;
                    if (child.activeSelf)
                        DestroyImmediate(child);
                }

                var template = transform.GetChild(0)
                    .gameObject;

                foreach (var cmd in enumeration)
                {
                    var instance = _instantiator.InstantiatePrefab(template, transform);
                    instance.SetActive(true);
                    var contextScript = instance.GetComponent<IContext>();
                    contextScript.Value = cmd;
                }
            });
        }

        /// <summary>
        ///     This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }
}