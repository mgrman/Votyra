namespace Votyra.Core.Models
{
    /// <summary>
    ///   <para>Representation of a plane in 3D space.</para>
    /// </summary>
    public struct Plane3f
    {
        private readonly Vector3f m_Normal;
        private float m_Distance;

        /// <summary>
        ///   <para>Normal vector of the plane.</para>
        /// </summary>
        public Vector3f normal
        {
            get
            {
                return this.m_Normal;
            }
        }

        /// <summary>
        ///   <para>Distance from the origin to the plane.</para>
        /// </summary>
        public float distance
        {
            get
            {
                return this.m_Distance;
            }
        }

        /// <summary>
        ///   <para>Creates a plane.</para>
        /// </summary>
        /// <param name="inNormal"></param>
        /// <param name="inPoint"></param>
        public Plane3f(Vector3f inNormal, Vector3f inPoint)
        {
            this.m_Normal = inNormal.normalized;
            this.m_Distance = -Vector3f.Dot(inNormal, inPoint);
        }

        /// <summary>
        ///   <para>Creates a plane.</para>
        /// </summary>
        /// <param name="inNormal"></param>
        /// <param name="d"></param>
        public Plane3f(Vector3f inNormal, float d)
        {
            this.m_Normal = inNormal.normalized;
            this.m_Distance = d;
        }

        /// <summary>
        ///   <para>Creates a plane.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public Plane3f(Vector3f a, Vector3f b, Vector3f c)
        {
            this.m_Normal = Vector3f.Cross(b - a, c - a).normalized;
            this.m_Distance = -Vector3f.Dot(this.m_Normal, a);
        }


        /// <summary>
        ///   <para>Returns a signed distance from plane to point.</para>
        /// </summary>
        /// <param name="inPt"></param>
        public float GetDistanceToPoint(Vector3f inPt)
        {
            return Vector3f.Dot(this.normal, inPt) + this.distance;
        }

        /// <summary>
        ///   <para>Is a point on the positive side of the plane?</para>
        /// </summary>
        /// <param name="inPt"></param>
        public bool GetSide(Vector3f inPt)
        {
            return (double)Vector3f.Dot(this.normal, inPt) + (double)this.distance > 0.0;
        }

        /// <summary>
        ///   <para>Are two points on the same side of the plane?</para>
        /// </summary>
        /// <param name="inPt0"></param>
        /// <param name="inPt1"></param>
        public bool SameSide(Vector3f inPt0, Vector3f inPt1)
        {
            float distanceToPoint1 = this.GetDistanceToPoint(inPt0);
            float distanceToPoint2 = this.GetDistanceToPoint(inPt1);
            return (double)distanceToPoint1 > 0.0 && (double)distanceToPoint2 > 0.0 || (double)distanceToPoint1 <= 0.0 && (double)distanceToPoint2 <= 0.0;
        }

    }
}