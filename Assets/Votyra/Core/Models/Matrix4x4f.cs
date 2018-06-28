using System;

namespace Votyra.Core.Models
{
    /// <summary>
    ///   <para>A standard 4x4 transformation matrix.</para>
    /// </summary>
    public struct Matrix4x4f
    {
        public static readonly Matrix4x4f zero = new Matrix4x4f(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);

        public static readonly Matrix4x4f identity = new Matrix4x4f(1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f);

        public readonly float m00;
        public readonly float m10;
        public readonly float m20;
        public readonly float m30;
        public readonly float m01;
        public readonly float m11;
        public readonly float m21;
        public readonly float m31;
        public readonly float m02;
        public readonly float m12;
        public readonly float m22;
        public readonly float m32;
        public readonly float m03;
        public readonly float m13;
        public readonly float m23;
        public readonly float m33;

        public Matrix4x4f(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33)
        {
            this.m00 = m00;
            this.m10 = m10;
            this.m20 = m20;
            this.m30 = m30;
            this.m01 = m01;
            this.m11 = m11;
            this.m21 = m21;
            this.m31 = m31;
            this.m02 = m02;
            this.m12 = m12;
            this.m22 = m22;
            this.m32 = m32;
            this.m03 = m03;
            this.m13 = m13;
            this.m23 = m23;
            this.m33 = m33;
        }

        public static Matrix4x4f operator *(Matrix4x4f lhs, Matrix4x4f rhs)
        {
            float m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30;
            float m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31;
            float m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32;
            float m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33;
            float m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30;
            float m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31;
            float m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32;
            float m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33;
            float m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30;
            float m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31;
            float m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32;
            float m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33;
            float m30 = lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30;
            float m31 = lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31;
            float m32 = lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32;
            float m33 = lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33;
            return new Matrix4x4f(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33);
        }

        public static Vector4f operator *(Matrix4x4f lhs, Vector4f v)
        {
            float x = (float)((double)lhs.m00 * (double)v.X + (double)lhs.m01 * (double)v.Y + (double)lhs.m02 * (double)v.Z + (double)lhs.m03 * (double)v.W);
            float y = (float)((double)lhs.m10 * (double)v.X + (double)lhs.m11 * (double)v.Y + (double)lhs.m12 * (double)v.Z + (double)lhs.m13 * (double)v.W);
            float z = (float)((double)lhs.m20 * (double)v.X + (double)lhs.m21 * (double)v.Y + (double)lhs.m22 * (double)v.Z + (double)lhs.m23 * (double)v.W);
            float w = (float)((double)lhs.m30 * (double)v.X + (double)lhs.m31 * (double)v.Y + (double)lhs.m32 * (double)v.Z + (double)lhs.m33 * (double)v.W);
            return new Vector4f(x, y, z, w);
        }

        public Vector4f GetColumn(int i)
        {
            switch (i)
            {
                case 0:
                    return new Vector4f(m00, m10, m20, m30);

                case 1:
                    return new Vector4f(m01, m11, m21, m31);

                case 2:
                    return new Vector4f(m02, m12, m22, m32);

                case 3:
                    return new Vector4f(m03, m13, m23, m33);

                default:
                    throw new InvalidOperationException($"Unsuported column '{i}'! Column must be between 0-3.");
            }
        }

        public Vector3f MultiplyPoint(Vector3f v)
        {
            float x = (float)((double)this.m00 * (double)v.X + (double)this.m01 * (double)v.Y + (double)this.m02 * (double)v.Z) + this.m03;
            float y = (float)((double)this.m10 * (double)v.X + (double)this.m11 * (double)v.Y + (double)this.m12 * (double)v.Z) + this.m13;
            float z = (float)((double)this.m20 * (double)v.X + (double)this.m21 * (double)v.Y + (double)this.m22 * (double)v.Z) + this.m23;
            float num = 1f / ((float)((double)this.m30 * (double)v.X + (double)this.m31 * (double)v.Y + (double)this.m32 * (double)v.Z) + this.m33);
            x *= num;
            y *= num;
            z *= num;
            return new Vector3f(x, y, z);
        }

        public Vector3f MultiplyPoint3x4(Vector3f v)
        {
            float x = (float)((double)this.m00 * (double)v.X + (double)this.m01 * (double)v.Y + (double)this.m02 * (double)v.Z) + this.m03;
            float y = (float)((double)this.m10 * (double)v.X + (double)this.m11 * (double)v.Y + (double)this.m12 * (double)v.Z) + this.m13;
            float z = (float)((double)this.m20 * (double)v.X + (double)this.m21 * (double)v.Y + (double)this.m22 * (double)v.Z) + this.m23;
            return new Vector3f(x, y, z);
        }

        public Vector3f MultiplyVector(Vector3f v)
        {
            float x = (float)((double)this.m00 * (double)v.X + (double)this.m01 * (double)v.Y + (double)this.m02 * (double)v.Z);
            float y = (float)((double)this.m10 * (double)v.X + (double)this.m11 * (double)v.Y + (double)this.m12 * (double)v.Z);
            float z = (float)((double)this.m20 * (double)v.X + (double)this.m21 * (double)v.Y + (double)this.m22 * (double)v.Z);
            return new Vector3f(x, y, z);
        }

        public float Determinant
        {
            get
            {
                //https://stackoverflow.com/questions/2937702/i-want-to-find-determinant-of-4x4-matrix-in-c-sharp
                return
                    m03 * m12 * m21 * m30 - m02 * m13 * m21 * m30 -
                    m03 * m11 * m22 * m30 + m01 * m13 * m22 * m30 +
                    m02 * m11 * m23 * m30 - m01 * m12 * m23 * m30 -
                    m03 * m12 * m20 * m31 + m02 * m13 * m20 * m31 +
                    m03 * m10 * m22 * m31 - m00 * m13 * m22 * m31 -
                    m02 * m10 * m23 * m31 + m00 * m12 * m23 * m31 +
                    m03 * m11 * m20 * m32 - m01 * m13 * m20 * m32 -
                    m03 * m10 * m21 * m32 + m00 * m13 * m21 * m32 +
                    m01 * m10 * m23 * m32 - m00 * m11 * m23 * m32 -
                    m02 * m11 * m20 * m33 + m01 * m12 * m20 * m33 +
                    m02 * m10 * m21 * m33 - m00 * m12 * m21 * m33 -
                    m01 * m10 * m22 * m33 + m00 * m11 * m22 * m33;
            }
        }

        public Matrix4x4f Inverse
        {
            get
            {
                return Matrix4x4fInvertor.Invert(this);
            }
        }

        public static Matrix4x4f Scale(Vector3f v)
        {
            return new Matrix4x4f(
                v.X, 0.0f, 0.0f, 0.0f,
                0.0f, v.Y, 0.0f, 0.0f,
                0.0f, 0.0f, v.Z, 0.0f,
                0.0f, 0.0f, 0.0f, 1.0f);
        }

        public static Matrix4x4f Translate(Vector3f v)
        {
            return new Matrix4x4f(
                1.0f, 0.0f, 0.0f, v.X,
                0.0f, 1.0f, 0.0f, v.Y,
                0.0f, 0.0f, 1.0f, v.Z,
                0.0f, 0.0f, 0.0f, 1.0f);
        }

        public static Matrix4x4f Rotate(Quaternion4f q)
        {
            float num = q.x * 2f;
            float num2 = q.y * 2f;
            float num3 = q.z * 2f;
            float num4 = q.x * num;
            float num5 = q.y * num2;
            float num6 = q.z * num3;
            float num7 = q.x * num2;
            float num8 = q.x * num3;
            float num9 = q.y * num3;
            float num10 = q.w * num;
            float num11 = q.w * num2;
            float num12 = q.w * num3;
            float m00 = 1f - (num5 + num6);
            float m10 = num7 + num12;
            float m20 = num8 - num11;
            float m30 = 0f;
            float m01 = num7 - num12;
            float m11 = 1f - (num4 + num6);
            float m21 = num9 + num10;
            float m31 = 0f;
            float m02 = num8 + num11;
            float m12 = num9 - num10;
            float m22 = 1f - (num4 + num5);
            float m32 = 0f;
            float m03 = 0f;
            float m13 = 0f;
            float m23 = 0f;
            float m33 = 1f;
            return new Matrix4x4f(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33);
        }

        public static bool operator ==(Matrix4x4f lhs, Matrix4x4f rhs)
        {
            return lhs.GetColumn(0) == rhs.GetColumn(0) && lhs.GetColumn(1) == rhs.GetColumn(1) && lhs.GetColumn(2) == rhs.GetColumn(2) && lhs.GetColumn(3) == rhs.GetColumn(3);
        }

        public static bool operator !=(Matrix4x4f lhs, Matrix4x4f rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.GetColumn(0).GetHashCode() ^ this.GetColumn(1).GetHashCode() << 2 ^ this.GetColumn(2).GetHashCode() >> 2 ^ this.GetColumn(3).GetHashCode() >> 1;
            }
        }

        public override bool Equals(object other)
        {
            if (!(other is Matrix4x4f))
                return false;
            Matrix4x4f that = (Matrix4x4f)other;
            return this == that;
        }

        public override string ToString()
        {
            return string.Format("{0:F5}\t{1:F5}\t{2:F5}\t{3:F5}\n{4:F5}\t{5:F5}\t{6:F5}\t{7:F5}\n{8:F5}\t{9:F5}\t{10:F5}\t{11:F5}\n{12:F5}\t{13:F5}\t{14:F5}\t{15:F5}\n", this.m00, this.m01, this.m02, this.m03, this.m10, this.m11, this.m12, this.m13, this.m20, this.m21, this.m22, this.m23, this.m30, this.m31, this.m32, this.m33);
        }
    }
}