using System;
using UnityEngine;

namespace Votyra.Core.Images
{
    public class StaticLogConfig : MonoBehaviour
    {
        [SerializeField]
        private LogLevel logLevel;

        public LogLevel LogLevel => this.logLevel;

        public static StaticLogConfig Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Multiple instances of StaticLogConfig!!");
                return;
            }

            Instance = this;
        }
    }
}
