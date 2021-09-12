using System;

namespace Votyra.Core.Models
{
    /// <summary>
    ///     <para>A standard 4x4 transformation matrix.</para>
    /// </summary>
    public struct Matrix4x4f
    {
        public static readonly Matrix4x4f Zero = new Matrix4x4f(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

        public static readonly Matrix4x4f Identity = new Matrix4x4f(1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f);

        public readonly float M00;
        public readonly float M10;
        public readonly float M20;
        public readonly float M30;
        public readonly float M01;
        public readonly float M11;
        public readonly float M21;
        public readonly float M31;
        public readonly float M02;
        public readonly float M12;
        public readonly float M22;
        public readonly float M32;
        public readonly float M03;
        public readonly float M13;
        public readonly float M23;
        public readonly float M33;

        public Matrix4x4f(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33)
        {
            this.M00 = m00;
            this.M10 = m10;
            this.M20 = m20;
            this.M30 = m30;
            this.M01 = m01;
            this.M11 = m11;
            this.M21 = m21;
            this.M31 = m31;
            this.M02 = m02;
            this.M12 = m12;
            this.M22 = m22;
            this.M32 = m32;
            this.M03 = m03;
            this.M13 = m13;
            this.M23 = m23;
            this.M33 = m33;
        }

        public float Determinant => M03 * M12 * M21 * M30 - M02 * M13 * M21 * M30 - M03 * M11 * M22 * M30 + M01 * M13 * M22 * M30 + M02 * M11 * M23 * M30 - M01 * M12 * M23 * M30 - M03 * M12 * M20 * M31 + M02 * M13 * M20 * M31 + M03 * M10 * M22 * M31 - M00 * M13 * M22 * M31 - M02 * M10 * M23 * M31 + M00 * M12 * M23 * M31 + M03 * M11 * M20 * M32 - M01 * M13 * M20 * M32 - M03 * M10 * M21 * M32 + M00 * M13 * M21 * M32 + M01 * M10 * M23 * M32 - M00 * M11 * M23 * M32 - M02 * M11 * M20 * M33 + M01 * M12 * M20 * M33 + M02 * M10 * M21 * M33 - M00 * M12 * M21 * M33 - M01 * M10 * M22 * M33 + M00 * M11 * M22 * M33;

        public Matrix4x4f Inverse => this.Invert();

        public static Matrix4x4f operator *(Matrix4x4f lhs, Matrix4x4f rhs)
        {
            var m00 = lhs.M00 * rhs.M00 + lhs.M01 * rhs.M10 + lhs.M02 * rhs.M20 + lhs.M03 * rhs.M30;
            var m01 = lhs.M00 * rhs.M01 + lhs.M01 * rhs.M11 + lhs.M02 * rhs.M21 + lhs.M03 * rhs.M31;
            var m02 = lhs.M00 * rhs.M02 + lhs.M01 * rhs.M12 + lhs.M02 * rhs.M22 + lhs.M03 * rhs.M32;
            var m03 = lhs.M00 * rhs.M03 + lhs.M01 * rhs.M13 + lhs.M02 * rhs.M23 + lhs.M03 * rhs.M33;
            var m10 = lhs.M10 * rhs.M00 + lhs.M11 * rhs.M10 + lhs.M12 * rhs.M20 + lhs.M13 * rhs.M30;
            var m11 = lhs.M10 * rhs.M01 + lhs.M11 * rhs.M11 + lhs.M12 * rhs.M21 + lhs.M13 * rhs.M31;
            var m12 = lhs.M10 * rhs.M02 + lhs.M11 * rhs.M12 + lhs.M12 * rhs.M22 + lhs.M13 * rhs.M32;
            var m13 = lhs.M10 * rhs.M03 + lhs.M11 * rhs.M13 + lhs.M12 * rhs.M23 + lhs.M13 * rhs.M33;
            var m20 = lhs.M20 * rhs.M00 + lhs.M21 * rhs.M10 + lhs.M22 * rhs.M20 + lhs.M23 * rhs.M30;
            var m21 = lhs.M20 * rhs.M01 + lhs.M21 * rhs.M11 + lhs.M22 * rhs.M21 + lhs.M23 * rhs.M31;
            var m22 = lhs.M20 * rhs.M02 + lhs.M21 * rhs.M12 + lhs.M22 * rhs.M22 + lhs.M23 * rhs.M32;
            var m23 = lhs.M20 * rhs.M03 + lhs.M21 * rhs.M13 + lhs.M22 * rhs.M23 + lhs.M23 * rhs.M33;
            var m30 = lhs.M30 * rhs.M00 + lhs.M31 * rhs.M10 + lhs.M32 * rhs.M20 + lhs.M33 * rhs.M30;
            var m31 = lhs.M30 * rhs.M01 + lhs.M31 * rhs.M11 + lhs.M32 * rhs.M21 + lhs.M33 * rhs.M31;
            var m32 = lhs.M30 * rhs.M02 + lhs.M31 * rhs.M12 + lhs.M32 * rhs.M22 + lhs.M33 * rhs.M32;
            var m33 = lhs.M30 * rhs.M03 + lhs.M31 * rhs.M13 + lhs.M32 * rhs.M23 + lhs.M33 * rhs.M33;
            return new Matrix4x4f(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33);
        }

        public static Vector4f operator *(Matrix4x4f lhs, Vector4f v)
        {
            var x = (float) (lhs.M00 * (double) v.X + lhs.M01 * (double) v.Y + lhs.M02 * (double) v.Z + lhs.M03 * (double) v.W);
            var y = (float) (lhs.M10 * (double) v.X + lhs.M11 * (double) v.Y + lhs.M12 * (double) v.Z + lhs.M13 * (double) v.W);
            var z = (float) (lhs.M20 * (double) v.X + lhs.M21 * (double) v.Y + lhs.M22 * (double) v.Z + lhs.M23 * (double) v.W);
            var w = (float) (lhs.M30 * (double) v.X + lhs.M31 * (double) v.Y + lhs.M32 * (double) v.Z + lhs.M33 * (double) v.W);
            return new Vector4f(x, y, z, w);
        }

        public static Matrix4x4f Scale(Vector3f v) => new Matrix4x4f(v.X, 0.0f, 0.0f, 0.0f, 0.0f, v.Y, 0.0f, 0.0f, 0.0f, 0.0f, v.Z, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f);

        public static Matrix4x4f Translate(Vector3f v) => new Matrix4x4f(1.0f, 0.0f, 0.0f, v.X, 0.0f, 1.0f, 0.0f, v.Y, 0.0f, 0.0f, 1.0f, v.Z, 0.0f, 0.0f, 0.0f, 1.0f);

        public static Matrix4x4f Rotate(Quaternion4f q)
        {
            var num = q.X * 2f;
            var num2 = q.Y * 2f;
            var num3 = q.Z * 2f;
            var num4 = q.X * num;
            var num5 = q.Y * num2;
            var num6 = q.Z * num3;
            var num7 = q.X * num2;
            var num8 = q.X * num3;
            var num9 = q.Y * num3;
            var num10 = q.W * num;
            var num11 = q.W * num2;
            var num12 = q.W * num3;
            var m00 = 1f - (num5 + num6);
            var m10 = num7 + num12;
            var m20 = num8 - num11;
            var m30 = 0f;
            var m01 = num7 - num12;
            var m11 = 1f - (num4 + num6);
            var m21 = num9 + num10;
            var m31 = 0f;
            var m02 = num8 + num11;
            var m12 = num9 - num10;
            var m22 = 1f - (num4 + num5);
            var m32 = 0f;
            var m03 = 0f;
            var m13 = 0f;
            var m23 = 0f;
            var m33 = 1f;
            return new Matrix4x4f(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33);
        }

        public static bool operator ==(Matrix4x4f lhs, Matrix4x4f rhs) => lhs.GetColumn(0) == rhs.GetColumn(0) && lhs.GetColumn(1) == rhs.GetColumn(1) && lhs.GetColumn(2) == rhs.GetColumn(2) && lhs.GetColumn(3) == rhs.GetColumn(3);

        public static bool operator !=(Matrix4x4f lhs, Matrix4x4f rhs) => !(lhs == rhs);

        public Vector4f GetColumn(int i)
        {
            switch (i)
            {
                case 0:
                    return new Vector4f(M00, M10, M20, M30);

                case 1:
                    return new Vector4f(M01, M11, M21, M31);

                case 2:
                    return new Vector4f(M02, M12, M22, M32);

                case 3:
                    return new Vector4f(M03, M13, M23, M33);

                default:
                    throw new InvalidOperationException($"Unsuported column '{i}'! Column must be between 0-3.");
            }
        }

        public Vector3f MultiplyPoint(Vector3f v)
        {
            var x = (float) (M00 * (double) v.X + M01 * (double) v.Y + M02 * (double) v.Z) + M03;
            var y = (float) (M10 * (double) v.X + M11 * (double) v.Y + M12 * (double) v.Z) + M13;
            var z = (float) (M20 * (double) v.X + M21 * (double) v.Y + M22 * (double) v.Z) + M23;
            var num = 1f / ((float) (M30 * (double) v.X + M31 * (double) v.Y + M32 * (double) v.Z) + M33);
            x *= num;
            y *= num;
            z *= num;
            return new Vector3f(x, y, z);
        }

        public Vector3f MultiplyPoint3X4(Vector3f v)
        {
            var x = (float) (M00 * (double) v.X + M01 * (double) v.Y + M02 * (double) v.Z) + M03;
            var y = (float) (M10 * (double) v.X + M11 * (double) v.Y + M12 * (double) v.Z) + M13;
            var z = (float) (M20 * (double) v.X + M21 * (double) v.Y + M22 * (double) v.Z) + M23;
            return new Vector3f(x, y, z);
        }

        public Vector3f MultiplyVector(Vector3f v)
        {
            var x = (float) (M00 * (double) v.X + M01 * (double) v.Y + M02 * (double) v.Z);
            var y = (float) (M10 * (double) v.X + M11 * (double) v.Y + M12 * (double) v.Z);
            var z = (float) (M20 * (double) v.X + M21 * (double) v.Y + M22 * (double) v.Z);
            return new Vector3f(x, y, z);
        }

        public override int GetHashCode() =>
            GetColumn(0)
                .GetHashCode() ^ (GetColumn(1)
                .GetHashCode() << 2) ^ (GetColumn(2)
                .GetHashCode() >> 2) ^ (GetColumn(3)
                .GetHashCode() >> 1);

        public override bool Equals(object other)
        {
            if (!(other is Matrix4x4f))
                return false;
            var that = (Matrix4x4f) other;
            return this == that;
        }

        public override string ToString() => string.Format("{0:F5}\t{1:F5}\t{2:F5}\t{3:F5}\n{4:F5}\t{5:F5}\t{6:F5}\t{7:F5}\n{8:F5}\t{9:F5}\t{10:F5}\t{11:F5}\n{12:F5}\t{13:F5}\t{14:F5}\t{15:F5}\n", M00, M01, M02, M03, M10, M11, M12, M13, M20, M21, M22, M23, M30, M31, M32, M33);
    }
}