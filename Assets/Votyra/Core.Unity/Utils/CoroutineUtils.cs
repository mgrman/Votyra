using System.Collections;
using UnityEngine;

namespace Votyra.Core.Utils
{
    public static class CoroutineUtils
    {
        public static void StartCoroutine(IEnumerator coroutine)
        {
            CoroutineRunner.Instance.StartCoroutine(coroutine);
        }

        private class CoroutineRunner : MonoBehaviour
        {
            private static CoroutineRunner instance;

            public static CoroutineRunner Instance
            {
                get
                {
                    if (instance == null)
                    {
                        instance = new GameObject(nameof(CoroutineRunner)).AddComponent<CoroutineRunner>();
                    }

                    return instance;
                }
            }
        }
    }
}
