using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.Raycasting
{
    public class AggregatorRaycaster : IRaycaster, IRaycasterAggregator
    {
        private readonly List<IRaycaster> _raycasters = new List<IRaycaster>();

        public Vector3f Raycast(Ray3f cameraRay)
        {
            foreach (var raycaster in _raycasters)
            {
                var res = raycaster.Raycast(cameraRay);
                if (res.NoNan())
                {
                    return res;
                }
            }

            return Vector3f.NaN;
        }

        public float Raycast(Vector2f cameraRay)
        {
            foreach (var raycaster in _raycasters)
            {
                var res = raycaster.Raycast(cameraRay);
                if (res.NoNan())
                {
                    return res;
                }
            }

            return float.NaN;
        }

        public void Attach(IRaycaster raycaster)
        {
            _raycasters.Add(raycaster);
        }
    }
}
