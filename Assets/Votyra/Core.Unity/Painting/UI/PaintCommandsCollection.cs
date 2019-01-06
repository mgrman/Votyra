using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

namespace Votyra.Core.Painting.UI
{
    public class PaintCommandsCollection : MonoBehaviour
    {
        [Inject]
        protected IInstantiator _instantiator;

        [Inject]
        protected IPaintingModel _paintingModel;

        public string PaintCommands;

        // Start is called before the first frame update
        private void Start()
        {
            //_paintingModel.PaintCommands
            _paintingModel.PaintCommands.Subscribe(o =>
            {
                PaintCommands = string.Join(", ", o.Select(o1 => o1.GetType()
                    .Name));
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

                foreach (var cmd in o)
                {
                    var instance = _instantiator.InstantiatePrefab(template, transform);
                    instance.SetActive(true);
                    var instanceScript = instance.GetComponentInChildren<PaintCommandButton>();
                    instanceScript.PaintCommand = cmd;
                }
            });
        }
    }
}