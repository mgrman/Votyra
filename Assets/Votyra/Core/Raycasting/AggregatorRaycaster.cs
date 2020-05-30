using System.Collections.Generic;
using Votyra.Core.Models;

namespace Votyra.Core.Raycasting
{
    public class AggregatorRaycaster : IRaycasterAggregator
    {
        private readonly List<IRaycasterPart> raycasters = new List<IRaycasterPart>();

        public Vector3f Raycast(Ray3f cameraRay)
        {
            foreach (var raycaster in this.raycasters)
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
            foreach (var raycaster in this.raycasters)
            {
                var res = raycaster.Raycast(cameraRay);
                if (res.NoNan())
                {
                    return res;
                }
            }

            return float.NaN;
        }

        public void Attach(IRaycasterPart raycaster)
        {
            this.raycasters.Add(raycaster);
        }
    }
}
