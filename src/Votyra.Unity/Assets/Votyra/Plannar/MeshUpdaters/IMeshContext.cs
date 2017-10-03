using System;
using Votyra.Models;
using UnityEngine;
using Votyra.Profiling;
using Votyra.Logging;

namespace Votyra.Plannar.MeshUpdaters
{
    public interface IMeshContext : IContext
    {
        Func<GameObject> GameObjectFactory { get; }
    }
}
