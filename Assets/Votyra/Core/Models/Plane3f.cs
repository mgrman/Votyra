namespace Votyra.Core.Models
{
    public struct Plane3f
    {
        public readonly Vector3f Normal;

        public readonly float Distance;

        public Plane3f(Vector3f inNormal, float d)
        {
            this.Normal = inNormal.Normalized;
            this.Distance = d;
        }

        public float GetDistanceToPoint(Vector3f inPt)
        {
            return Vector3f.Dot(this.Normal, inPt) + this.Distance;
        }
    }
}