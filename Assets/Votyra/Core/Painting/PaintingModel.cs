using System.Collections.Generic;
using Votyra.Core.Logging;
using Votyra.Core.Painting.Commands;

namespace Votyra.Core.Painting
{
    public class PaintingModel : IPaintingModel
    {
        public PaintingModel(List<IPaintCommandFactory> paintCommandFactories)
        {
            this.PaintCommandFactories = paintCommandFactories;
        }

        public IReadOnlyList<IPaintCommandFactory> PaintCommandFactories { get; }
    }
}