using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Votyra.Core.Pooling;
using Votyra.Core.Utils;
using Zenject;

namespace Votyra.Core
{
    public class PoolStats : MonoBehaviour
    {
        public IEnumerable<IPool> Pools { get; private set; }

        [Inject]
        public void Initialize(List<IPool> pools)
        {
            Pools = pools;
        }
    }
}