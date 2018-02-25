using System;

using UnityEngine;
using Votyra.Core;

namespace Votyra.Plannar.MeshUpdaters
{
    public interface IMeshContext : IContext
    {
        Func<GameObject> GameObjectFactory { get; }
    }
}