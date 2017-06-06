using System;
using Votyra.Common.Models;
using UnityEngine;

namespace Votyra.Unity.MeshUpdaters
{
    public interface IMeshContext
    {
        Func<GameObject> GameObjectFactory { get; }
    }
}