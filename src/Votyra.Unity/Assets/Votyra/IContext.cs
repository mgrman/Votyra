using System;
using UnityEngine;
using Votyra.Profiling;
using Votyra.Logging;

namespace Votyra
{
    public interface IContext
    {
        ProfilerFactoryDelegate ProfilerFactory { get; }
        LoggerFactoryDelegate LoggerFactory { get; }
    }
}