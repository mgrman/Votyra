using UnityEngine;

namespace Votyra.Core
{
    public interface IMaterialConfig : ISharedConfig
    {
        Material Material { get; }

        Material MaterialWalls { get; }
    }
}
