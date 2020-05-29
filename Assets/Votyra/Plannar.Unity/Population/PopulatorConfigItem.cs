using System;
using UnityEngine;
using Votyra.Core.Models;

namespace Votyra.Plannar.Unity
{
    [Serializable]
    public class PopulatorConfigItem
    {
        [SerializeField]
        private uint countPerGroup;

        [SerializeField]
        private AnimationCurve heightCurve;

        [SerializeField]
        private Material material;

        [SerializeField]
        private Mesh mesh;

        [SerializeField]
        private Area1f uniformScaleVariance;

        public uint CountPerGroup => this.countPerGroup;

        public AnimationCurve HeightCurve => this.heightCurve;

        public Material Material => this.material;

        public Mesh Mesh => this.mesh;

        public Area1f UniformScaleVariance => this.uniformScaleVariance;
    }
}
