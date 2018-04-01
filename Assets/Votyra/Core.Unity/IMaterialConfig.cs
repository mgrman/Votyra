using UnityEngine;

namespace Votyra.Core
{
    public interface IMaterialConfig
    {
        Material Material { get; }
        Material MaterialWalls { get; }
    }
}