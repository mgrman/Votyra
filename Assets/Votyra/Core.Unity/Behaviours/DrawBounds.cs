using UnityEngine;
using Votyra.Core.Utils;

namespace Votyra.Core.Behaviours
{
    public class DrawBounds : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            var bounds = gameObject.GetComponent<MeshFilter>()
                .sharedMesh.bounds;

            bounds = transform.TransformBounds(bounds);
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}