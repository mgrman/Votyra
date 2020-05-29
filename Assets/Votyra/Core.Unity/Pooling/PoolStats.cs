using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Pooling;
using Zenject;

namespace Votyra.Core
{
    public class PoolStats : MonoBehaviour
    {
        public IEnumerable<IPool> Pools { get; private set; }

        [Inject,]
        public void Initialize(List<IPool> pools)
        {
            this.Pools = pools;
        }
    }
}
