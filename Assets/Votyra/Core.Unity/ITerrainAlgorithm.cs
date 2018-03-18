using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Models;
using System;
using UniRx;

namespace Votyra.Core
{
    public interface ITerrainAlgorithm
    {
        string Name { get; }
        GameObject Prefab { get; }
    }
}