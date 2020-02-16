using System.Collections.Generic;
using Votyra.Core.Painting.Commands;

namespace Votyra.Core.Painting
{
    public interface IPaintingModel
    {
        IReadOnlyList<IPaintCommandFactory> PaintCommandFactories { get; }
    }
}