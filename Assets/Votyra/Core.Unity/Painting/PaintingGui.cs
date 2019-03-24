using UnityEngine;
using Votyra.Core.Painting;
using Zenject;

namespace Votyra.Core.Unity.Painting
{
    public class PaintingGui : MonoBehaviour
    {
        [Inject]
        public IPaintingModel PaintingModel;
    }
}