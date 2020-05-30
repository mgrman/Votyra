using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public struct SampledData3B : IEquatable<SampledData3B>
    {
        public const int MaskBitShiftX0Y0Z0 = (int)MaskBitShift.X0Y0Z0;

        public const int MaskBitShiftX0Y0Z1 = (int)MaskBitShift.X0Y0Z1;

        public const int MaskBitShiftX0Y1Z0 = (int)MaskBitShift.X0Y1Z0;

        public const int MaskBitShiftX0Y1Z1 = (int)MaskBitShift.X0Y1Z1;

        public const int MaskBitShiftX1Y0Z0 = (int)MaskBitShift.X1Y0Z0;

        public const int MaskBitShiftX1Y0Z1 = (int)MaskBitShift.X1Y0Z1;

        public const int MaskBitShiftX1Y1Z0 = (int)MaskBitShift.X1Y1Z0;

        public const int MaskBitShiftX1Y1Z1 = (int)MaskBitShift.X1Y1Z1;

        public const int MaskX0Y0Z0 = (int)Mask.X0Y0Z0;

        public const int MaskX0Y0Z1 = (int)Mask.X0Y0Z1;

        public const int MaskX0Y1Z0 = (int)Mask.X0Y1Z0;

        public const int MaskX0Y1Z1 = (int)Mask.X0Y1Z1;

        public const int MaskX1Y0Z0 = (int)Mask.X1Y0Z0;

        public const int MaskX1Y0Z1 = (int)Mask.X1Y0Z1;

        public const int MaskX1Y1Z0 = (int)Mask.X1Y1Z0;

        public const int MaskX1Y1Z1 = (int)Mask.X1Y1Z1;

        public static readonly IEqualityComparer<SampledData3B> RotationInvariantNormallessComparer = new RotationInvariantNormallessSampledData3BComparer();

        public static readonly IEqualityComparer<SampledData3B> NormallessComparer = new NormallessSampledData3BComparer();

        public static IEnumerable<SampledData3B> AllValues = Enumerable.Range(0, byte.MaxValue + 1)
            .Select(o => new SampledData3B((byte)o))
            .ToArray();

        public readonly byte Data;

        private const string CubeRegex = @"[^0-9]*([0-9])[^0-9]*([0-9])[^0-9]*([0-9])[^0-9]*([0-9])[^0-9]*([0-9])[^0-9]*([0-9])[^0-9]*([0-9])[^0-9]*([0-9])[^0-9]*";

        public SampledData3B(bool x0Y0Z0, bool x0Y0Z1, bool x0Y1Z0, bool x0Y1Z1, bool x1Y0Z0, bool x1Y0Z1, bool x1Y1Z0, bool x1Y1Z1)
        {
            this.Data = (byte)((x0Y0Z0 ? MaskX0Y0Z0 : 0) | (x0Y0Z1 ? MaskX0Y0Z1 : 0) | (x0Y1Z0 ? MaskX0Y1Z0 : 0) | (x0Y1Z1 ? MaskX0Y1Z1 : 0) | (x1Y0Z0 ? MaskX1Y0Z0 : 0) | (x1Y0Z1 ? MaskX1Y0Z1 : 0) | (x1Y1Z0 ? MaskX1Y1Z0 : 0) | (x1Y1Z1 ? MaskX1Y1Z1 : 0));
        }

        public SampledData3B(byte data)
        {
            this.Data = data;
        }

        public enum MaskBitShift
        {
            X0Y0Z0 = 0,
            X0Y0Z1 = 1,
            X0Y1Z0 = 2,
            X0Y1Z1 = 3,
            X1Y0Z0 = 4,
            X1Y0Z1 = 5,
            X1Y1Z0 = 6,
            X1Y1Z1 = 7,
        }

        public enum Mask : byte
        {
            X0Y0Z0 = 1 << MaskBitShift.X0Y0Z0,
            X0Y0Z1 = 1 << MaskBitShift.X0Y0Z1,
            X0Y1Z0 = 1 << MaskBitShift.X0Y1Z0,
            X0Y1Z1 = 1 << MaskBitShift.X0Y1Z1,
            X1Y0Z0 = 1 << MaskBitShift.X1Y0Z0,
            X1Y0Z1 = 1 << MaskBitShift.X1Y0Z1,
            X1Y1Z0 = 1 << MaskBitShift.X1Y1Z0,
            X1Y1Z1 = 1 << MaskBitShift.X1Y1Z1,
        }

        public bool DataX0Y0Z0 => (this.Data & MaskX0Y0Z0) != 0;

        public bool DataX0Y0Z1 => (this.Data & MaskX0Y0Z1) != 0;

        public bool DataX0Y1Z0 => (this.Data & MaskX0Y1Z0) != 0;

        public bool DataX0Y1Z1 => (this.Data & MaskX0Y1Z1) != 0;

        public bool DataX1Y0Z0 => (this.Data & MaskX1Y0Z0) != 0;

        public bool DataX1Y0Z1 => (this.Data & MaskX1Y0Z1) != 0;

        public bool DataX1Y1Z0 => (this.Data & MaskX1Y1Z0) != 0;

        public bool DataX1Y1Z1 => (this.Data & MaskX1Y1Z1) != 0;

        public int TrueCount => this.NumberOfSetBits(this.Data);

        public bool this[Vector3i vec] => this[vec.X, vec.Y, vec.Z];

        public bool this[Vector3f vec] => this[vec.X.RoundToInt(), vec.Y.RoundToInt(), vec.Z.RoundToInt()];

        public bool this[int x, int y, int z] => (this.Data & (1 << ((x * 4) + (y * 2) + z))) != 0;

        public static Matrix4X4F GetRotationMatrix(Vector3i rotationSteps, bool invert)
        {
            var rotationMatrix = Matrix4X4F.Rotate(Quaternion4F.Euler(rotationSteps.X * 90, rotationSteps.Y * 90, rotationSteps.Z * 90));
            var inversion = invert ? Matrix4X4F.Scale(new Vector3f(1, 1, -1)) : Matrix4X4F.Identity;

            var offsetInverted = Matrix4X4F.Translate(new Vector3f(-0.5f, -0.5f, -0.5f));
            var offset = Matrix4X4F.Translate(new Vector3f(0.5f, 0.5f, 0.5f));

            var finalMatrix = offset * inversion * rotationMatrix * offsetInverted;
            return finalMatrix;
        }

        public static bool operator ==(SampledData3B a, SampledData3B b) => a.Data == b.Data;

        public static bool operator !=(SampledData3B a, SampledData3B b) => a.Data != b.Data;

        public static SampledData3B ParseCube(string cube)
        {
            var match = Regex.Match(cube, CubeRegex);
            var valX0Y0Z0 = int.Parse(match.Groups[7]
                .Value) > 0;
            var valX0Y0Z1 = int.Parse(match.Groups[3]
                .Value) > 0;
            var valX0Y1Z0 = int.Parse(match.Groups[5]
                .Value) > 0;
            var valX0Y1Z1 = int.Parse(match.Groups[1]
                .Value) > 0;
            var valX1Y0Z0 = int.Parse(match.Groups[8]
                .Value) > 0;
            var valX1Y0Z1 = int.Parse(match.Groups[4]
                .Value) > 0;
            var valX1Y1Z0 = int.Parse(match.Groups[6]
                .Value) > 0;
            var valX1Y1Z1 = int.Parse(match.Groups[2]
                .Value) > 0;
            return new SampledData3B(valX0Y0Z0, valX0Y0Z1, valX0Y1Z0, valX0Y1Z1, valX1Y0Z0, valX1Y0Z1, valX1Y1Z0, valX1Y1Z1);
        }

        public IEnumerable<Vector3i> GetPointsWithValue(bool value)
        {
            if (this.DataX0Y0Z0 == value)
            {
                yield return new Vector3i(0, 0, 0);
            }

            if (this.DataX0Y0Z1 == value)
            {
                yield return new Vector3i(0, 0, 1);
            }

            if (this.DataX0Y1Z0 == value)
            {
                yield return new Vector3i(0, 1, 0);
            }

            if (this.DataX0Y1Z1 == value)
            {
                yield return new Vector3i(0, 1, 1);
            }

            if (this.DataX1Y0Z0 == value)
            {
                yield return new Vector3i(1, 0, 0);
            }

            if (this.DataX1Y0Z1 == value)
            {
                yield return new Vector3i(1, 0, 1);
            }

            if (this.DataX1Y1Z0 == value)
            {
                yield return new Vector3i(1, 1, 0);
            }

            if (this.DataX1Y1Z1 == value)
            {
                yield return new Vector3i(1, 1, 1);
            }
        }

        public SampledData3B GetRotatedXy(float angleDeg)
        {
            var rotationMatrix = Matrix4X4F.Rotate(Quaternion4F.Euler(0, 0, angleDeg));
            return this.GetTransformed(rotationMatrix);
        }

        public SampledData3B GetRotatedYz(float angleDeg)
        {
            var rotationMatrix = Matrix4X4F.Rotate(Quaternion4F.Euler(angleDeg, 0, 0));
            return this.GetTransformed(rotationMatrix);
        }

        public SampledData3B GetRotatedXz(float angleDeg)
        {
            var rotationMatrix = Matrix4X4F.Rotate(Quaternion4F.Euler(0, angleDeg, 0));
            return this.GetTransformed(rotationMatrix);
        }

        public SampledData3B GetRotated(Vector3i rotationSteps, bool invert)
        {
            var rotationMatrix = GetRotationMatrix(rotationSteps, invert);

            return this.GetTransformed(rotationMatrix);
        }

        public SampledData3B GetTransformed(Matrix4X4F matrix) => new SampledData3B(this[matrix.MultiplyPoint(new Vector3f(0, 0, 0))], this[matrix.MultiplyPoint(new Vector3f(0, 0, 1))], this[matrix.MultiplyPoint(new Vector3f(0, 1, 0))], this[matrix.MultiplyPoint(new Vector3f(0, 1, 1))], this[matrix.MultiplyPoint(new Vector3f(1, 0, 0))], this[matrix.MultiplyPoint(new Vector3f(1, 0, 1))], this[matrix.MultiplyPoint(new Vector3f(1, 1, 0))], this[matrix.MultiplyPoint(new Vector3f(1, 1, 1))]);

        public override bool Equals(object obj)
        {
            if (obj is SampledData3B)
            {
                var that = (SampledData3B)obj;
                return this.Equals(that);
            }

            return false;
        }

        public override int GetHashCode() => this.Data;

        public override string ToString() => this.ToCubeString();

        public bool Equals(SampledData3B that) => this == that;

        public bool EqualsRotationInvariant(SampledData3B that, out Matrix4X4F matrix, bool x = true, bool y = true, bool z = true, bool invert = true)
        {
            foreach (var tempMatrix in this.GetAllRotationMatrices(x, y, z, invert))
            {
                var rotatedThis = this.GetTransformed(tempMatrix);

                if (NormallessComparer.Equals(rotatedThis, that))
                {
                    matrix = tempMatrix;
                    return true;
                }
            }

            matrix = default;
            return false;
        }

        public bool IsContainedInRotationInvariant(SampledData3B that, out Matrix4X4F matrix, out SampledData3B rotatedData, bool x = true, bool y = true, bool z = true, bool invert = true)
        {
            foreach (var tempMatrix in this.GetAllRotationMatrices(x, y, z, invert))
            {
                var rotatedThis = this.GetTransformed(tempMatrix);

                if ((rotatedThis.Data & that.Data) == rotatedThis.Data)
                {
                    matrix = tempMatrix;
                    rotatedData = rotatedThis;
                    return true;
                }
            }

            matrix = default;
            rotatedData = default;
            return false;
        }

        public IEnumerable<Matrix4X4F> GetAllRotationMatrices(bool useX = true, bool useY = true, bool useZ = true, bool useInvert = true)
        {
            for (var x = 0; x < (useX ? 4 : 1); x++)
            {
                for (var y = 0; y < (useY ? 4 : 1); y++)
                {
                    for (var z = 0; z < (useZ ? 4 : 1); z++)
                    {
                        for (var invert = 0; invert < (useInvert ? 2 : 1); invert++)
                        {
                            yield return GetRotationMatrix(new Vector3i(x, y, z), invert == 1);
                        }
                    }
                }
            }
        }

        public IEnumerable<Matrix4X4F> GetAllRotationSubsets(SampledData3B that)
        {
            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    for (var z = 0; z < 4; z++)
                    {
                        for (var invert = 0; invert < 2; invert++)
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

        public string ToCubeString() => $@"
              {(this.DataX0Y1Z1 ? 1 : 0)}-----{(this.DataX1Y1Z1 ? 1 : 0)}
             /|    /|
            {(this.DataX0Y0Z1 ? 1 : 0)}-+---{(this.DataX1Y0Z1 ? 1 : 0)} |
            | {(this.DataX0Y1Z0 ? 1 : 0)}---+-{(this.DataX1Y1Z0 ? 1 : 0)}
            |/    |/
            {(this.DataX0Y0Z0 ? 1 : 0)}-----{(this.DataX1Y0Z0 ? 1 : 0)}
            ";

        private static Vector3f ComputeNormal(float x0Y0Z0, float x0Y0Z1, float x0Y1Z0, float x0Y1Z1, float x1Y0Z0, float x1Y0Z1, float x1Y1Z0, float x1Y1Z1)
        {
            var dxY0Z0 = x1Y0Z0 - x0Y0Z0;
            var dxY0Z1 = x1Y0Z1 - x0Y0Z1;
            var dxY0 = (dxY0Z0 + dxY0Z1) / 2;
            var dxY1Z0 = x1Y1Z0 - x0Y1Z0;
            var dxY1Z1 = x1Y1Z1 - x0Y1Z1;
            var dxY1 = (dxY1Z0 + dxY1Z1) / 2;
            var dx = (dxY0 + dxY1) / 2;

            var dyX0Z0 = x0Y1Z0 - x0Y0Z0;
            var dyX0Z1 = x0Y1Z1 - x0Y0Z1;
            var dyX0 = (dyX0Z0 + dyX0Z1) / 2;
            var dyX1Z0 = x1Y1Z0 - x1Y0Z0;
            var dyX1Z1 = x1Y1Z1 - x1Y0Z1;
            var dyX1 = (dyX1Z0 + dyX1Z1) / 2;
            var dy = (dyX0 + dyX1) / 2;

            var dzX0Y0 = x0Y0Z1 - x0Y0Z0;
            var dzX0Y1 = x0Y1Z1 - x0Y1Z0;
            var dzX0 = (dzX0Y0 + dzX0Y1) / 2;
            var dzX1Y0 = x1Y0Z1 - x1Y0Z0;
            var dzX1Y1 = x1Y1Z1 - x1Y1Z0;
            var dzX1 = (dzX1Y0 + dzX1Y1) / 2;
            var dz = (dzX0 + dzX1) / 2;

            return -new Vector3f(dx, dy, dz).Normalized();
        }

        private int NumberOfSetBits(int i)
        {
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }

        private bool GetIndexedY0ValueCw(int index)
        {
            switch (index % 4)
            {
                case 0:
                    return this.DataX0Y0Z0;

                case 1:
                    return this.DataX1Y0Z0;

                case 2:
                    return this.DataX1Y1Z0;

                case 3:
                    return this.DataX0Y1Z0;

                default:
                    throw new InvalidOperationException();
            }
        }

        private bool GetIndexedY1ValueCw(int index)
        {
            switch (index % 4)
            {
                case 0:
                    return this.DataX0Y0Z1;

                case 1:
                    return this.DataX1Y0Z1;

                case 2:
                    return this.DataX1Y1Z1;

                case 3:
                    return this.DataX0Y1Z1;

                default:
                    throw new InvalidOperationException();
            }
        }

        public class RotationInvariantNormallessSampledData3BComparer : IEqualityComparer<SampledData3B>
        {
            public bool Equals(SampledData3B x, SampledData3B y)
            {
                Matrix4X4F temp;
                return x.EqualsRotationInvariant(y, out temp);
            }

            public int GetHashCode(SampledData3B obj) => obj.TrueCount;
        }

        public class NormallessSampledData3BComparer : IEqualityComparer<SampledData3B>
        {
            public bool Equals(SampledData3B x, SampledData3B y) => x.Data == y.Data;

            public int GetHashCode(SampledData3B obj) => obj.Data;
        }
    }
}
