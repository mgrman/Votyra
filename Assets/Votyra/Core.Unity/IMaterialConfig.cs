using UnityEngine;

namespace Votyra.Core
{
    public interface IMaterialConfig : IConfig
    {
        Material Material { get; }
        
        Material MaterialWalls { get; }
    }
}