using System;
using Votyra.Models;
using UnityEngine;
using Votyra.Profiling;
using Votyra.Logging;

namespace Votyra.Cubical.MeshUpdaters
{
    public interface IMeshContext : IContext
    {
        Func<GameObject> GameObjectFactory { get; }
    }
}
