using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace Application.Scripts
{
    public class TestMesh : MonoBehaviour
    {
        public void Update()
        {
            var mesh = new Mesh();
            var array = Enumerable.Range(0, 10000).Select(o => new Vector3(o, o, o)).ToArray();
            for (int i = 0; i < 10000; i++)
            {
                Profiler.BeginSample("array");

                mesh.vertices = array;

                Profiler.EndSample();
            }

            var list = Enumerable.Range(0, 10000).Select(o => new Vector3(o, o, o)).ToList();
            for (int i = 0; i < 10000; i++)
            {
                Profiler.BeginSample("list");

                mesh.SetVertices(list);

                Profiler.EndSample();
            }
        }
    }
}