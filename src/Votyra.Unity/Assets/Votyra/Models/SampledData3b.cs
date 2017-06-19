using System;
using System.Collections.Generic;
using Votyra.Utils;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

namespace Votyra.Models
{

    public struct SampledData3b : IEquatable<SampledData3b>
    {
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
        public const int MaskBitShift_x0y0z0 = (int)MaskBitShift.x0y0z0;
        public const int MaskBitShift_x0y0z1 = (int)MaskBitShift.x0y0z1;
        public const int MaskBitShift_x0y1z0 = (int)MaskBitShift.x0y1z0;
        public const int MaskBitShift_x0y1z1 = (int)MaskBitShift.x0y1z1;
        public const int MaskBitShift_x1y0z0 = (int)MaskBitShift.x1y0z0;
        public const int MaskBitShift_x1y0z1 = (int)MaskBitShift.x1y0z1;
        public const int MaskBitShift_x1y1z0 = (int)MaskBitShift.x1y1z0;
        public const int MaskBitShift_x1y1z1 = (int)MaskBitShift.x1y1z1;

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

        public const int Mask_x0y0z0 = (int)Mask.x0y0z0;
        public const int Mask_x0y0z1 = (int)Mask.x0y0z1;
        public const int Mask_x0y1z0 = (int)Mask.x0y1z0;
        public const int Mask_x0y1z1 = (int)Mask.x0y1z1;
        public const int Mask_x1y0z0 = (int)Mask.x1y0z0;
        public const int Mask_x1y0z1 = (int)Mask.x1y0z1;
        public const int Mask_x1y1z0 = (int)Mask.x1y1z0;
        public const int Mask_x1y1z1 = (int)Mask.x1y1z1;

        public readonly byte Data;

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

        int NumberOfSetBits(int i)
        {
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }

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
        public SampledData3b(int x0y0z0, int x0y0z1, int x0y1z0, int x0y1z1, int x1y0z0, int x1y0z1, int x1y1z0, int x1y1z1)
        {
            Data = (byte)(
                x0y0z0 << MaskBitShift_x0y0z0 | //0
                x0y0z1 << MaskBitShift_x0y0z1 | //1
                x0y1z0 << MaskBitShift_x0y1z0 | //2
                x0y1z1 << MaskBitShift_x0y1z1 | //3
                x1y0z0 << MaskBitShift_x1y0z0 | //4
                x1y0z1 << MaskBitShift_x1y0z1 | //5
                x1y1z0 << MaskBitShift_x1y1z0 | //6
                x1y1z1 << MaskBitShift_x1y1z1); //7
        }

        public SampledData3b(byte data)
        {
            Data = data;
        }

        public bool this[Vector3i vec]
        {
            get
            {
                return this[vec.x, vec.y, vec.z];
            }
        }

        public bool this[Vector3 vec]
        {
            get
            {
                return this[vec.x.RoundToInt(), vec.y.RoundToInt(), vec.z.RoundToInt()];
            }
        }

        public bool this[int x, int y, int z]
        {
            get
            {
                return (Data & (1 << (x * 4 + y * 2 + z))) != 0;
            }
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


        // public SampledData3b GetRotatedXY(float angleDeg)
        // {

        //     var rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, angleDeg));
        //     return GetTransformed(rotationMatrix);
        // }

        // public SampledData3b GetRotatedYZ(float angleDeg)
        // {
        //     var rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(angleDeg, 0, 0));
        //     return GetTransformed(rotationMatrix);
        // }

        // public SampledData3b GetRotatedXZ(float angleDeg)
        // {
        //     var rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, angleDeg, 0));
        //     return GetTransformed(rotationMatrix);
        // }

        public SampledData3b GetRotated(Vector3i rotationSteps, bool invert)
        {
            var rotationMatrix = GetRotationMatrix(rotationSteps, invert);

            return GetTransformed(rotationMatrix);
        }

        public static Matrix4x4 GetRotationMatrix(Vector3i rotationSteps, bool invert)
        {
            var rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(rotationSteps.x * 90, rotationSteps.y * 90, rotationSteps.z * 90));
            var inversion = invert ? Matrix4x4.Scale(new Vector3(1, 1, -1)) : Matrix4x4.identity;

            var offsetInverted = Matrix4x4.Translate(new Vector3(-0.5f, -0.5f, -0.5f));
            var offset = Matrix4x4.Translate(new Vector3(0.5f, 0.5f, 0.5f));

            var finalMatrix = offset * inversion * rotationMatrix * offsetInverted;
            return finalMatrix;
        }

        public SampledData3b GetTransformed(Matrix4x4 matrix)
        {

            return new SampledData3b
            (
                this[matrix.MultiplyPoint(new Vector3(0, 0, 0))],

                this[matrix.MultiplyPoint(new Vector3(0, 0, 1))],

                this[matrix.MultiplyPoint(new Vector3(0, 1, 0))],

                this[matrix.MultiplyPoint(new Vector3(0, 1, 1))],

                this[matrix.MultiplyPoint(new Vector3(1, 0, 0))],

                this[matrix.MultiplyPoint(new Vector3(1, 0, 1))],

                this[matrix.MultiplyPoint(new Vector3(1, 1, 0))],

                this[matrix.MultiplyPoint(new Vector3(1, 1, 1))]
            );
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

        public override int GetHashCode()
        {
            return this.Data;
        }

        public override string ToString()
        {
            return Convert.ToString(Data, 2).PadLeft(8, '0');
        }

        public bool Equals(SampledData3b that)
        {
            return this == that;
        }

        public static bool operator ==(SampledData3b a, SampledData3b b)
        {
            return a.Data == b.Data;
        }

        public static bool operator !=(SampledData3b a, SampledData3b b)
        {
            return a.Data != b.Data;
        }

        public bool EqualsRotationInvariant(SampledData3b that, out Matrix4x4 matrix)
        {
            matrix = GetAllRotationEquivalencies(that).FirstOrDefault();

            return matrix != default(Matrix4x4);
        }

        public static readonly IEqualityComparer<SampledData3b> RotationInvariantComparer = new RotationInvariantSampledData3bComparer();

        public class RotationInvariantSampledData3bComparer : IEqualityComparer<SampledData3b>
        {
            public bool Equals(SampledData3b x, SampledData3b y)
            {
                Matrix4x4 temp;
                return x.EqualsRotationInvariant(y, out temp);
            }

            public int GetHashCode(SampledData3b obj)
            {
                return obj.TrueCount;
            }
        }

        public IEnumerable<Matrix4x4> GetAllRotationEquivalencies(SampledData3b that)
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
                            // Debug.Log($"{this} rotated by {x},{y},{z} equals {rotatedThis}");
                            if (rotatedThis == that)
                            {
                                yield return finalMatrix;
                            }
                        }
                    }
                }
            }
        }
        public IEnumerable<Matrix4x4> GetAllRotationSubsets(SampledData3b that)
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
                            // Debug.Log($"{this} rotated by {x},{y},{z} equals {rotatedThis}");
                            if ((rotatedThis.Data & that.Data) == rotatedThis.Data)
                            {
                                yield return finalMatrix;
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<SampledData3b> GenerateAllValues()
        {
            return Enumerable.Range(0, byte.MaxValue + 1).Select(o => new SampledData3b((byte)o));
        }

        const string CubeRegex = @"[^01]*([01])[^01]*([01])[^01]*([01])[^01]*([01])[^01]*([01])[^01]*([01])[^01]*([01])[^01]*([01])[^01]*";

        public static SampledData3b ParseCube(string cube)
        {
            var match = Regex.Match(cube, CubeRegex);
            bool val_x0y0z0 = int.Parse(match.Groups[7].Value) != 0;
            bool val_x0y0z1 = int.Parse(match.Groups[3].Value) != 0;
            bool val_x0y1z0 = int.Parse(match.Groups[5].Value) != 0;
            bool val_x0y1z1 = int.Parse(match.Groups[1].Value) != 0;
            bool val_x1y0z0 = int.Parse(match.Groups[8].Value) != 0;
            bool val_x1y0z1 = int.Parse(match.Groups[4].Value) != 0;
            bool val_x1y1z0 = int.Parse(match.Groups[6].Value) != 0;
            bool val_x1y1z1 = int.Parse(match.Groups[2].Value) != 0;
            return new SampledData3b(val_x0y0z0, val_x0y0z1, val_x0y1z0, val_x0y1z1, val_x1y0z0, val_x1y0z1, val_x1y1z0, val_x1y1z1);
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

    }
}