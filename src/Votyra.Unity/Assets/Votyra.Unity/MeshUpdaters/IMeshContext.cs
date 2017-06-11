using System;
using Votyra.Models;
using UnityEngine;

namespace Votyra.Unity.MeshUpdaters
{
    public interface IMeshContext
    {
        Func<GameObject> GameObjectFactory { get; }
    }
}