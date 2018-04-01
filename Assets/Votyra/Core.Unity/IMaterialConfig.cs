using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Votyra.Core.Models;
using Votyra.Core.Images.Constraints;
using System;

namespace Votyra.Core
{
    public interface IMaterialConfig
    {
        Material Material { get; }
        Material MaterialWalls { get; }
    }
}