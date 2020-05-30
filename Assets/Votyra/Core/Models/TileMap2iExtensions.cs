using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Logging;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public static class TileMap2IExtensions
    {
        public static TileMap2I CreateExpandedTileMap2I(this IEnumerable<SampledData2I> templates, float scaleFactor, IThreadSafeLogger logger) => templates.ScaleTemplates(scaleFactor)
            .CreateVariantsOfUmbra()
            .ExpandRotations()
            .CreateTileMap2I(logger);

        public static TileMap2I CreateTileMap2I(this IEnumerable<SampledData2I> templates, IThreadSafeLogger logger) => new TileMap2I(templates, logger);

        public static IEnumerable<SampledData2I> CreateVariantsOfUmbra(this IEnumerable<SampledData2I> templates)
        {
            return templates.SelectMany(t => CreateVariantsOfUmbra(t))
                .Distinct()
                .ToArray();
        }

        public static IEnumerable<SampledData2I> CreateVariantsOfUmbra(this SampledData2I tile)
        {
            var stepCountX0Y0 = tile.X0Y0.Abs();
            var stepCountX0Y1 = tile.X0Y1.Abs();
            var stepCountX1Y0 = tile.X1Y0.Abs();
            var stepCountX1Y1 = tile.X1Y1.Abs();
            var signX0Y0 = tile.X0Y0.Sign();
            var signX0Y1 = tile.X0Y1.Sign();
            var signX1Y0 = tile.X1Y0.Sign();
            var signX1Y1 = tile.X1Y1.Sign();

            for (var x0Y0 = 0; x0Y0 <= stepCountX0Y0; x0Y0++)
            {
                for (var x0Y1 = 0; x0Y1 <= stepCountX0Y1; x0Y1++)
                {
                    for (var x1Y0 = 0; x1Y0 <= stepCountX1Y0; x1Y0++)
                    {
                        for (var x1Y1 = 0; x1Y1 <= stepCountX1Y1; x1Y1++)
                        {
                            yield return new SampledData2I(x0Y0 * signX0Y0, x0Y1 * signX0Y1, x1Y0 * signX1Y0, x1Y1 * signX1Y1);
                        }
                    }
                }
            }
        }

        public static IEnumerable<SampledData2I> ExpandRotations(this IEnumerable<SampledData2I> templates)
        {
            return templates.SelectMany(template =>
                {
                    return new[]
                    {
                        template,
                        template.GetRotated(1),
                        template.GetRotated(2),
                        template.GetRotated(3),
                    };
                })
                .Distinct()
                .ToArray();
        }

        public static Area1i RangeUnion(this IEnumerable<SampledData2I> templates)
        {
            return templates.Select(o => o.Range)
                .Aggregate((Area1i?)null, (a, b) => a?.UnionWith(b) ?? b) ?? Area1i.Zero;
        }

        public static IEnumerable<SampledData2I> ScaleTemplates(this IEnumerable<SampledData2I> templates, float scale)
        {
            for (var i = 1; i <= scale; i += 1)
            {
                foreach (var template in templates)
                {
                    yield return template * i;
                }
            }
        }
    }
}
