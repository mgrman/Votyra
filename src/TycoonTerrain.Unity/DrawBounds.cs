using TycoonTerrain.Unity.Utils;
using UnityEngine;

[ExecuteInEditMode]
public class DrawBounds : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var bounds = this.gameObject.GetComponent<MeshFilter>().sharedMesh.bounds;

        bounds = this.transform.TransformBounds(bounds);
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}