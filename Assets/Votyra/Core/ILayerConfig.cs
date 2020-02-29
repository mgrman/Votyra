using System;
using Votyra.Core.Models;

namespace Votyra.Core
{
    public interface ILayerConfig : ISharedConfig
    {
        int Layer { get; }
    }
}