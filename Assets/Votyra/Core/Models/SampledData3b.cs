using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct SampledData3b : IEquatable<SampledData3b>
    {
        public const int Mask_x0y0z0 = (int)Mask.x0y0z0;

        public const int Mask_x0y0z1 = (int)Mask.x0y0z1;

        public const int Mask_x0y1z0 = (int)Mask.x0y1z0;

        public const int Mask_x0y1z1 = (int)Mask.x0y1z1;

        public const int Mask_x1y0z0 = (int)Mask.x1y0z0;

        public const int Mask_x1y0z1 = (int)Mask.x1y0z1;

        public const int Mask_x1y1z0 = (int)Mask.x1y1z0;

        public const int Mask_x1y1z1 = (int)Mask.x1y1z1;

        public const int MaskBitShift_x0y0z0 = (int)MaskBitShift.x0y0z0;

        public const int MaskBitShift_x0y0z1 = (int)MaskBitShift.x0y0z1;

        public const int MaskBitShift_x0y1z0 = (int)MaskBitShift.x0y1z0;

        public const int MaskBitShift_x0y1z1 = (int)MaskBitShift.x0y1z1;

        public const int MaskBitShift_x1y0z0 = (int)MaskBitShift.x1y0z0;

        public const int MaskBitShift_x1y0z1 = (int)MaskBitShift.x1y0z1;

        public const int MaskBitShift_x1y1z0 = (int)MaskBitShift.x1y1z0;

        public const int MaskBitShift_x1y1z1 = (int)MaskBitShift.x1y1z1;

        public static readonly IEqualityComparer<SampledData3b> NormallessComparer = new NormallessSampledData3bComparer();

        public static readonly IEqualityComparer<SampledData3b> RotationInvariantNormallessComparer = new RotationInvariantNormallessSampledData3bComparer();

        public static IEnumerable<SampledData3b> AllValues = Enumerable
            .Range(0, byte.MaxValue + 1)
            .Select(o => new SampledData3b((byte)o))
            .ToArray();

        public readonly byte Data;

        private const string CubeRegex = @"[^0-9]*([0-9])[^0-9]*([0-9])[^0-9]*([0-9])[^0-9]*([0-9])[^0-9]*([0-9])[^0-9]*([0-9])[^0-9]*([0-9])[^0-9]*([0-9])[^0-9]*";

        public SampledData3b(bool x0y0z0, bool x0y0z1, bool x0y1z0, bool x0y1z1, bool x1y0z0, bool x1y0z1, bool x1y1z0, bool x1y1z1)
        {
            Data = (byte)(
                (x0y0z0 ? Mask_x0y0z0 : 0) |
                (x0y0z1 ? Mask_x0y0z1 : 0) |
                (x0y1z0 ? Mask_x0y1z0 : 0) |
                (x0y1z1 ? Mask_x0y1z1 : 0) |
                (x1y0z0 ? Mask_x1y0z0 : 0) |
                (x1y0z1 ? Mask_x1y0z1 : 0) |
                (x1y1z0 ? Mask_x1y1z0 : 0) |
                (x1y1z1 ? Mask_x1y1z1 : 0));
        }

        public SampledData3b(byte data)
        {
            Data = data;
        }

        public enum Mask : byte
        {
            x0y0z0 = 1 << MaskBitShift.x0y0z0,
            x0y0z1 = 1 << MaskBitShift.x0y0z1,
            x0y1z0 = 1 << MaskBitShift.x0y1z0,
            x0y1z1 = 1 << MaskBitShift.x0y1z1,
            x1y0z0 = 1 << MaskBitShift.x1y0z0,
            x1y0z1 = 1 << MaskBitShift.x1y0z1,
            x1y1z0 = 1 << MaskBitShift.x1y1z0,
            x1y1z1 = 1 << MaskBitShift.x1y1z1
        }

        public enum MaskBitShift : int
        {
            x0y0z0 = 0,
            x0y0z1 = 1,
            x0y1z0 = 2,
            x0y1z1 = 3,
            x1y0z0 = 4,
            x1y0z1 = 5,
            x1y1z0 = 6,
            x1y1z1 = 7
        }

        public bool Data_x0y0z0 { get { return (Data & Mask_x0y0z0) != 0; } }
        public bool Data_x0y0z1 { get { return (Data & Mask_x0y0z1) != 0; } }
        public bool Data_x0y1z0 { get { return (Data & Mask_x0y1z0) != 0; } }
        public bool Data_x0y1z1 { get { return (Data & Mask_x0y1z1) != 0; } }
        public bool Data_x1y0z0 { get { return (Data & Mask_x1y0z0) != 0; } }
        public bool Data_x1y0z1 { get { return (Data & Mask_x1y0z1) != 0; } }
        public bool Data_x1y1z0 { get { return (Data & Mask_x1y1z0) != 0; } }
        public bool Data_x1y1z1 { get { return (Data & Mask_x1y1z1) != 0; } }

        public int TrueCount
        {
            get
            {
                return NumberOfSetBits(Data);
            }
        }

        public bool this[Vector3i vec]
        {
            get
            {
                return this[vec.X, vec.Y, vec.Z];
            }
        }

        public bool this[Vector3f vec]
        {
            get
            {
                return this[vec.X.RoundToInt(), vec.Y.RoundToInt(), vec.Z.RoundToInt()];
            }
        }

        public bool this[int x, int y, int z]
        {
            get
            {
                return (Data & (1 << (x * 4 + y * 2 + z))) != 0;
            }
        }

        public static Matrix4x4f GetRotationMatrix(Vector3i rotationSteps, bool invert)
        {
            var rotationMatrix = Matrix4x4f.Rotate(Quaternion4f.Euler(rotationSteps.X * 90, rotationSteps.Y * 90, rotationSteps.Z * 90));
            var inversion = invert ? Matrix4x4f.Scale(new Vector3f(1, 1, -1)) : Matrix4x4f.identity;

            var offsetInverted = Matrix4x4f.Translate(new Vector3f(-0.5f, -0.5f, -0.5f));
            var offset = Matrix4x4f.Translate(new Vector3f(0.5f, 0.5f, 0.5f));

            var finalMatrix = offset * inversion * rotationMatrix * offsetInverted;
            return finalMatrix;
        }

        public static bool operator !=(SampledData3b a, SampledData3b b)
        {
            return a.Data != b.Data;
        }

        public static bool operator ==(SampledData3b a, SampledData3b b)
        {
            return a.Data == b.Data;
        }

        public static SampledData3b ParseCube(string cube)
        {
            var match = Regex.Match(cube, CubeRegex);
            bool val_x0y0z0 = int.Parse(match.Groups[7].Value) > 0;
            bool val_x0y0z1 = int.Parse(match.Groups[3].Value) > 0;
            bool val_x0y1z0 = int.Parse(match.Groups[5].Value) > 0;
            bool val_x0y1z1 = int.Parse(match.Groups[1].Value) > 0;
            bool val_x1y0z0 = int.Parse(match.Groups[8].Value) > 0;
            bool val_x1y0z1 = int.Parse(match.Groups[4].Value) > 0;
            bool val_x1y1z0 = int.Parse(match.Groups[6].Value) > 0;
            bool val_x1y1z1 = int.Parse(match.Groups[2].Value) > 0;
            return new SampledData3b(val_x0y0z0, val_x0y0z1, val_x0y1z0, val_x0y1z1, val_x1y0z0, val_x1y0z1, val_x1y1z0, val_x1y1z1);
        }

        public override bool Equals(object obj)
        {
            if (obj is SampledData3b)
            {
                var that = (SampledData3b)obj;
                return this.Equals(that);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(SampledData3b that)
        {
            return this == that;
        }

        public bool EqualsRotationInvariant(SampledData3b that, out Matrix4x4f matrix, bool x = true, bool y = true, bool z = true, bool invert = true)
        {
            foreach (var tempMatrix in GetAllRotationMatrices(x, y, z, invert))
            {
                var rotatedThis = this.GetTransformed(tempMatrix);

                if (NormallessComparer.Equals(rotatedThis, that))
                {
                    matrix = tempMatrix;
                    return true;
                }
            }

            matrix = default(Matrix4x4f);
            return false;
        }

        public IEnumerable<Matrix4x4f> GetAllRotationMatrices(bool useX = true, bool useY = true, bool useZ = true, bool useInvert = true)
        {
            for (int x = 0; x < (useX ? 4 : 1); x++)
            {
                for (int y = 0; y < (useY ? 4 : 1); y++)
                {
                    for (int z = 0; z < (useZ ? 4 : 1); z++)
                    {
                        for (int invert = 0; invert < (useInvert ? 2 : 1); invert++)
                        {
                            yield return GetRotationMatrix(new Vector3i(x, y, z), invert == 1);
                        }
                    }
                }
            }
        }

        public IEnumerable<Matrix4x4f> GetAllRotationSubsets(SampledData3b that)
        {
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    for (int z = 0; z < 4; z++)
                    {
                        for (int invert = 0; invert < 2; invert++)
                        {
                            var finalMatrix = GetRotationMatrix(new Vector3i(x, y, z), invert == 1);

                            var rotatedThis = this.GetTransformed(finalMatrix);

                            if ((rotatedThis.Data & that.Data) == rotatedThis.Data)
                            {
                                yield return finalMatrix;
                            }
                        }
                    }
                }
            }
        }

        public override int GetHashCode()
        {
            return this.Data;
        }

        public IEnumerable<Vector3i> GetPointsWithValue(bool value)
        {
            if (Data_x0y0z0 == value)
                yield return new Vector3i(0, 0, 0);
            if (Data_x0y0z1 == value)
                yield return new Vector3i(0, 0, 1);
            if (Data_x0y1z0 == value)
                yield return new Vector3i(0, 1, 0);
            if (Data_x0y1z1 == value)
                yield return new Vector3i(0, 1, 1);
            if (Data_x1y0z0 == value)
                yield return new Vector3i(1, 0, 0);
            if (Data_x1y0z1 == value)
                yield return new Vector3i(1, 0, 1);
            if (Data_x1y1z0 == value)
                yield return new Vector3i(1, 1, 0);
            if (Data_x1y1z1 == value)
                yield return new Vector3i(1, 1, 1);
        }

        public SampledData3b GetRotated(Vector3i rotationSteps, bool invert)
        {
            var rotationMatrix = GetRotationMatrix(rotationSteps, invert);

            return GetTransformed(rotationMatrix);
        }

        public SampledData3b GetRotatedXY(float angleDeg)
        {
            var rotationMatrix = Matrix4x4f.Rotate(Quaternion4f.Euler(0, 0, angleDeg));
            return GetTransformed(rotationMatrix);
        }

        public SampledData3b GetRotatedXZ(float angleDeg)
        {
            var rotationMatrix = Matrix4x4f.Rotate(Quaternion4f.Euler(0, angleDeg, 0));
            return GetTransformed(rotationMatrix);
        }

        public SampledData3b GetRotatedYZ(float angleDeg)
        {
            var rotationMatrix = Matrix4x4f.Rotate(Quaternion4f.Euler(angleDeg, 0, 0));
            return GetTransformed(rotationMatrix);
        }

        public SampledData3b GetTransformed(Matrix4x4f matrix)
        {
            return new SampledData3b
            (
                this[matrix.MultiplyPoint(new Vector3f(0, 0, 0))],

                this[matrix.MultiplyPoint(new Vector3f(0, 0, 1))],

                this[matrix.MultiplyPoint(new Vector3f(0, 1, 0))],

                this[matrix.MultiplyPoint(new Vector3f(0, 1, 1))],

                this[matrix.MultiplyPoint(new Vector3f(1, 0, 0))],

                this[matrix.MultiplyPoint(new Vector3f(1, 0, 1))],

                this[matrix.MultiplyPoint(new Vector3f(1, 1, 0))],

                this[matrix.MultiplyPoint(new Vector3f(1, 1, 1))]
            );
        }

        public bool IsContainedInRotationInvariant(SampledData3b that, out Matrix4x4f matrix, out SampledData3b rotatedData, bool x = true, bool y = true, bool z = true, bool invert = true)
        {
            foreach (var tempMatrix in GetAllRotationMatrices(x, y, z, invert))
            {
                var rotatedThis = this.GetTransformed(tempMatrix);

                if ((rotatedThis.Data & that.Data) == rotatedThis.Data)
                {
                    matrix = tempMatrix;
                    rotatedData = rotatedThis;
                    return true;
                }
            }

            matrix = default(Matrix4x4f);
            rotatedData = default(SampledData3b);
            return false;
        }

        public string ToCubeString()
        {
            return $@"
              {(this.Data_x0y1z1 ? 1 : 0)}-----{(this.Data_x1y1z1 ? 1 : 0)}
             /|    /|
            {(this.Data_x0y0z1 ? 1 : 0)}-+---{(this.Data_x1y0z1 ? 1 : 0)} |
            | {(this.Data_x0y1z0 ? 1 : 0)}---+-{(this.Data_x1y1z0 ? 1 : 0)}
            |/    |/
            {(this.Data_x0y0z0 ? 1 : 0)}-----{(this.Data_x1y0z0 ? 1 : 0)}
            ";
        }

        public override string ToString()
        {
            return ToCubeString();
            //            return Convert.ToString(Data, 2).PadLeft(8, '0');
        }

        private static Vector3f ComputeNormal(float x0y0z0, float x0y0z1, float x0y1z0, float x0y1z1, float x1y0z0, float x1y0z1, float x1y1z0, float x1y1z1)
        {
            var dx_y0z0 = x1y0z0 - x0y0z0;
            var dx_y0z1 = x1y0z1 - x0y0z1;
            var dx_y0 = (dx_y0z0 + dx_y0z1) / 2;
            var dx_y1z0 = x1y1z0 - x0y1z0;
            var dx_y1z1 = x1y1z1 - x0y1z1;
            var dx_y1 = (dx_y1z0 + dx_y1z1) / 2;
            var dx = (dx_y0 + dx_y1) / 2;

            var dy_x0z0 = x0y1z0 - x0y0z0;
            var dy_x0z1 = x0y1z1 - x0y0z1;
            var dy_x0 = (dy_x0z0 + dy_x0z1) / 2;
            var dy_x1z0 = x1y1z0 - x1y0z0;
            var dy_x1z1 = x1y1z1 - x1y0z1;
            var dy_x1 = (dy_x1z0 + dy_x1z1) / 2;
            var dy = (dy_x0 + dy_x1) / 2;

            var dz_x0y0 = x0y0z1 - x0y0z0;
            var dz_x0y1 = x0y1z1 - x0y1z0;
            var dz_x0 = (dz_x0y0 + dz_x0y1) / 2;
            var dz_x1y0 = x1y0z1 - x1y0z0;
            var dz_x1y1 = x1y1z1 - x1y1z0;
            var dz_x1 = (dz_x1y0 + dz_x1y1) / 2;
            var dz = (dz_x0 + dz_x1) / 2;

            return -new Vector3f(dx, dy, dz).Normalized;
        }

        private bool GetIndexedY0ValueCW(int index)
        {
            switch (index % 4)
            {
                case 0:
                    return Data_x0y0z0;

                case 1:
                    return Data_x1y0z0;

                case 2:
                    return Data_x1y1z0;

                case 3:
                    return Data_x0y1z0;

                default:
                    throw new InvalidOperationException();
            }
        }

        private bool GetIndexedY1ValueCW(int index)
        {
            switch (index % 4)
            {
                case 0:
                    return Data_x0y0z1;

                case 1:
                    return Data_x1y0z1;

                case 2:
                    return Data_x1y1z1;

                case 3:
                    return Data_x0y1z1;

                default:
                    throw new InvalidOperationException();
            }
        }

        private int NumberOfSetBits(int i)
        {
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }

        public class NormallessSampledData3bComparer : IEqualityComparer<SampledData3b>
        {
            public bool Equals(SampledData3b x, SampledData3b y)
            {
                return x.Data == y.Data;
            }

            public int GetHashCode(SampledData3b obj)
            {
                return obj.Data;
            }
        }

        public class RotationInvariantNormallessSampledData3bComparer : IEqualityComparer<SampledData3b>
        {
            public bool Equals(SampledData3b x, SampledData3b y)
            {
                Matrix4x4f temp;
                return x.EqualsRotationInvariant(y, out temp);
            }

            public int GetHashCode(SampledData3b obj)
            {
                return obj.TrueCount;
            }
        }
    }
}