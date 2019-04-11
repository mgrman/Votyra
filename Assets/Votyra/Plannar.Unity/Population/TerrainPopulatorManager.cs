using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Votyra.Core;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.Utils;
using Votyra.Core.TerrainMeshes;
using Zenject;
using Random = System.Random;

namespace Votyra.Plannar.Unity
{
    public class TerrainPopulatorManager : MonoBehaviour
    {
        [Inject]
        public void Initialize(ITerrainGeneratorManager2i manager, ITerrainConfig config, IPopulatorConfig populatorConfig, [Inject(Id = "root")] GameObject root)
        {
            foreach (var configItem in populatorConfig.ConfigItems)
            {
                var go=new GameObject();
                go.transform.SetParent(transform);
                var populator=go.AddComponent<TerrainPopulator>();
                populator.Initialize(manager,config,configItem,root);
            }
        }
    }
}