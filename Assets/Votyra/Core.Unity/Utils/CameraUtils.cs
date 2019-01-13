using System;
using System.Threading;
using UnityEngine;

namespace Votyra.Core.Utils
{
    public static class CameraUtils
    {
        private static readonly Lazy<Camera> _mainCamera = new Lazy<Camera>(() =>
        {
            Camera camera = null;
            TaskUtils.RunOnMainThread(() => camera = Camera.main);
            return camera;
        }, LazyThreadSafetyMode.ExecutionAndPublication);

        public static Camera MainCamera => _mainCamera.Value;
    }
}