using UnityEngine;

namespace Votyra.Core.Unity.Config
{
    public interface IMaterialConfig : IConfig
    {
        Material Material { get; }
        
        Material MaterialWalls { get; }
    }
}