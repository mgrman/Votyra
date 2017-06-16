using System;
using System.Collections.Generic;
using Votyra.Utils;
using UnityEngine;
using System.Linq;

namespace Votyra.Models
{

    public struct SampledData3b : IEquatable<SampledData3b>
    {
        public enum Mask : byte
        {
            x0y0z0 = 1 << 0,
            x0y0z1 = 1 << 1,
            x0y1z0 = 1 << 2,
            x0y1z1 = 1 << 3,
            x1y0z0 = 1 << 4,
            x1y0z1 = 1 << 5,
            x1y1z0 = 1 << 6,
            x1y1z1 = 1 << 7
        }

        public const byte Mask_x0y0z0 = (byte)Mask.x0y0z0;
        public const byte Mask_x0y0z1 = (byte)Mask.x0y0z1;
        public const byte Mask_x0y1z0 = (byte)Mask.x0y1z0;
        public const byte Mask_x0y1z1 = (byte)Mask.x0y1z1;
        public const byte Mask_x1y0z0 = (byte)Mask.x1y0z0;
        public const byte Mask_x1y0z1 = (byte)Mask.x1y0z1;
        public const byte Mask_x1y1z0 = (byte)Mask.x1y1z0;
        public const byte Mask_x1y1z1 = (byte)Mask.x1y1z1;

        public readonly byte Data;
        public bool Data_x0y0z0 { get { return (Data & Mask_x0y0z0) != 0; } }
        public bool Data_x0y0z1 { get { return (Data & Mask_x0y0z1) != 0; } }
        public bool Data_x0y1z0 { get { return (Data & Mask_x0y1z0) != 0; } }
        public bool Data_x0y1z1 { get { return (Data & Mask_x0y1z1) != 0; } }
        public bool Data_x1y0z0 { get { return (Data & Mask_x1y0z0) != 0; } }
        public bool Data_x1y0z1 { get { return (Data & Mask_x1y0z1) != 0; } }
        public bool Data_x1y1z0 { get { return (Data & Mask_x1y1z0) != 0; } }
        public bool Data_x1y1z1 { get { return (Data & Mask_x1y1z1) != 0; } }

        // public SampledData3b(bool x0y0z0, bool x0y0z1, bool x0y1z0, bool x0y1z1, bool x1y0z0, bool x1y0z1, bool x1y1z0, bool x1y1z1)
        // {
        //     Data = (byte)(
        //         (x0y0z0 ? Mask_x0y0z0 : 0) |
        //         (x0y0z1 ? Mask_x0y0z1 : 0) |
        //         (x0y1z0 ? Mask_x0y1z0 : 0) |
        //         (x0y1z1 ? Mask_x0y1z1 : 0) |
        //         (x1y0z0 ? Mask_x1y0z0 : 0) |
        //         (x1y0z1 ? Mask_x1y0z1 : 0) |
        //         (x1y1z0 ? Mask_x1y1z0 : 0) |
        //         (x1y1z1 ? Mask_x1y1z1 : 0));

        // }

        public SampledData3b(byte data)
        {
            Data = data;
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


        public static IEnumerable<SampledData3b> GenerateAllValues()
        {
            return Enumerable.Range(byte.MinValue, byte.MinValue + byte.MaxValue).Select(o => new SampledData3b((byte)o));
        }
    }
}