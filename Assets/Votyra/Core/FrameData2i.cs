using System;
using System.Collections.Generic;
using Votyra.Core.Images;
using Votyra.Core.Models;
using Votyra.Core.Pooling;

namespace Votyra.Core
{
    public class FrameData2i : IFrameData2i,IPoolable<FrameData2i>
    {
        private int _activeCounter;
        private IImage2f _image;
        private IMask2e _mask;

        public FrameData2i()
        {
            CameraPlanes = new Plane3f[6];
            CameraFrustumCorners = new Vector3f[4];
        }

        public Ray3f CameraRay { get; set; }
        public Plane3f[] CameraPlanes { get; }
        public Vector3f[] CameraFrustumCorners { get; }

        IReadOnlyList<Plane3f> IFrameData.CameraPlanes => CameraPlanes;
        IReadOnlyList<Vector3f> IFrameData.CameraFrustumCorners => CameraFrustumCorners;

        public void Activate()
        {
            _activeCounter++;
        }

        public void Deactivate()
        {
            _activeCounter--;
            if (_activeCounter <= 0)
                Dispose();
        }

        public Area1f RangeZ { get; private set; }
        public Range2i InvalidatedArea { get; set; }
        public Vector2i CellInGroupCount { get; set; }
        public int MeshSubdivision { get; set; }

        public IImage2f Image
        {
            get => _image;
            set
            {
                (_image as IInitializableImage)?.FinishUsing();
                _image = value;
                (_image as IInitializableImage)?.StartUsing();
                RangeZ = _image?.RangeZ ?? Area1f.Zero;
            }
        }

        public IMask2e Mask
        {
            get => _mask;
            set
            {
                (_mask as IInitializableImage)?.FinishUsing();
                _mask = value;
                (_mask as IInitializableImage)?.StartUsing();
            }
        }

        public void Dispose()
        {
            Image = null;
            Mask = null;
            OnDispose?.Invoke(this);
        }

        public event Action<FrameData2i> OnDispose;
    }
}