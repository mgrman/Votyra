using System;
using System.Collections;
using System.Threading.Tasks;
using UniRx;
using UniRx.Async;
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
            private static CoroutineRunner _instance;

            public static CoroutineRunner Instance
            {
                get
                {
                    if (_instance == null)
                        _instance = new GameObject(nameof(CoroutineRunner)).AddComponent<CoroutineRunner>();

                    return _instance;
                }
            }
        }
    }
}