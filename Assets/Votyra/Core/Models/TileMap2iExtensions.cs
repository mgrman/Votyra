using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Logging;
using Votyra.Core.Utils;

namespace Votyra.Core.Models
{
    public static class TileMap2iExtensions
    {
        public static TileMap2i CreateExpandedTileMap2i(this IEnumerable<SampledData2i> templates, float scaleFactor, IThreadSafeLogger logger) => templates.ScaleTemplates(scaleFactor)
            .CreateVariantsOfUmbra()
            .ExpandRotations()
            .CreateTileMap2i(logger);

        public static TileMap2i CreateTileMap2i(this IEnumerable<SampledData2i> templates, IThreadSafeLogger logger) => new TileMap2i(templates, logger);

        public static IEnumerable<SampledData2i> CreateVariantsOfUmbra(this IEnumerable<SampledData2i> templates)
        {
            return templates.SelectMany(t => CreateVariantsOfUmbra(t))
                .Distinct()
                .ToArray();
        }

        public static IEnumerable<SampledData2i> CreateVariantsOfUmbra(this SampledData2i tile)
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
                            yield return new SampledData2i(x0Y0 * signX0Y0, x0Y1 * signX0Y1, x1Y0 * signX1Y0, x1Y1 * signX1Y1);
                        }
                    }
                }
            }
        }

        public static IEnumerable<SampledData2i> ExpandRotations(this IEnumerable<SampledData2i> templates)
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

        public static Area1i RangeUnion(this IEnumerable<SampledData2i> templates)
        {
            return templates.Select(o => o.Range)
                .Aggregate((Area1i?)null, (a, b) => a?.UnionWith(b) ?? b) ?? Area1i.Zero;
        }

        public static IEnumerable<SampledData2i> ScaleTemplates(this IEnumerable<SampledData2i> templates, float scale)
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
