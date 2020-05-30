using System;
using UnityEngine;
using Votyra.Core.Models;

namespace Votyra.Plannar.Unity
{
    public class PopulatorConfigItem
    {
        public uint CountPerGroup { get; set; }
    

        public AnimationCurve HeightCurve { get; set; }

        public Material Material { get; set; }

        public Mesh Mesh { get; set; }

        public Area1f UniformScaleVariance { get; set; }
    }
}
