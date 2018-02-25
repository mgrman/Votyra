using System;

using UnityEngine;
using Votyra.Core;

namespace Votyra.Cubical.MeshUpdaters
{
    public interface IMeshContext : IContext
    {
        Func<GameObject> GameObjectFactory { get; }
    }
}