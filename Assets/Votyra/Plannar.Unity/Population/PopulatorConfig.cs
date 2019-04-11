using System;
using UnityEngine;
using Votyra.Core;
using Votyra.Core.Models;

namespace Votyra.Plannar.Unity
{
    public interface IPopulatorConfig : IConfig
    {
        PopulatorConfigItem[] ConfigItems { get; }
    }

    public class PopulatorConfig: IPopulatorConfig
    {
        public PopulatorConfig([ConfigInject("populationConfigItems")] PopulatorConfigItem[] configItems)
        {
            ConfigItems = configItems;
        }

        public PopulatorConfigItem[] ConfigItems { get; }
    }

    [Serializable]
    public class PopulatorConfigItem
    {
        [SerializeField]
        public Mesh Mesh;

        [SerializeField]
        public Material Material;

        [SerializeField]
        public uint CountPerGroup;
        // public Area1f UniformScaleVariance { get; set; }

    }
}