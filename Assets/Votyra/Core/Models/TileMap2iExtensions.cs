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
            var stepCount_x0y0 = tile.x0y0.Abs();
            var stepCount_x0y1 = tile.x0y1.Abs();
            var stepCount_x1y0 = tile.x1y0.Abs();
            var stepCount_x1y1 = tile.x1y1.Abs();
            var sign_x0y0 = tile.x0y0.Sign();
            var sign_x0y1 = tile.x0y1.Sign();
            var sign_x1y0 = tile.x1y0.Sign();
            var sign_x1y1 = tile.x1y1.Sign();

            for (var x0y0 = 0; x0y0 <= stepCount_x0y0; x0y0++)
            {
                for (var x0y1 = 0; x0y1 <= stepCount_x0y1; x0y1++)
                {
                    for (var x1y0 = 0; x1y0 <= stepCount_x1y0; x1y0++)
                    {
                        for (var x1y1 = 0; x1y1 <= stepCount_x1y1; x1y1++)
                        {
                            yield return new SampledData2i(x0y0 * sign_x0y0, x0y1 * sign_x0y1, x1y0 * sign_x1y0, x1y1 * sign_x1y1);
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
