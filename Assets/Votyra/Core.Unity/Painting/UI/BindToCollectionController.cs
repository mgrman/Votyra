using System;
using UniRx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Linq;

namespace Votyra.Core.Painting.UI
{
    public class BindToCollectionController : MonoBehaviour
    {
        public string TypeName;

        public string PropertyName;

        [Inject]
        protected IInstantiator _instantiator;

        [Inject]
        protected DiContainer _diContainer;

        private IDisposable _subscription;

        // Start is called before the first frame update
        private void Start()
        {
            var type = System.Type.GetType(TypeName, false);
            var propertyInfo = type.GetProperty(PropertyName);
            var model = _diContainer.Resolve(type);
            var observableProperty = propertyInfo.GetValue(model) as IObservable<object>;

            _subscription = observableProperty.Subscribe(o =>
            {
                var enumeration = o as IEnumerable<object>;

                var childrenCount = transform.childCount;
                for (int i = childrenCount - 1; i >= 0; i--)
                {
                    var child = transform.GetChild(i).gameObject;
                    if (child.activeSelf)
                    {
                        GameObject.DestroyImmediate(child);
                    }
                }

                var template = transform.GetChild(0).gameObject;

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
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }
}