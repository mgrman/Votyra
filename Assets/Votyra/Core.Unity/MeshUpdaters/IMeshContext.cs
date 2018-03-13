using System;
using UnityEngine;
using Votyra.Core;

namespace Votyra.Core.MeshUpdaters
{
    public interface IMeshContext : IContext
    {
        Func<GameObject> GameObjectFactory { get; }
    }
}