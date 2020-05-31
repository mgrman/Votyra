using System;
using System.Threading;
using UnityEngine;

namespace Votyra.Core.Utils
{
    public static class CameraUtils
    {
        private static readonly Lazy<Camera> LazyMainCamera = new Lazy<Camera>(MainCameraFactory, LazyThreadSafetyMode.ExecutionAndPublication);

        public static Camera MainCamera => LazyMainCamera.Value;

        private static Camera MainCameraFactory()
        {
            Camera camera = null;
            MainThreadUtils.RunOnMainThread(() => camera = Camera.main);
            return camera;
        }
    }
}
