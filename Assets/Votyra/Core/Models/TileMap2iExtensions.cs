using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public static class TileMap2iExtensions
    {
        public static TileMap2i CreateExpandedTileMap2i(this IEnumerable<SampledData2hi> templates, int scaleFactor)
        {
            return templates
                .ScaleTemplates(scaleFactor)
                .CreateVariantsOfUmbra()
                .ExpandRotations()
                .CreateTileMap2i();
        }

        public static TileMap2i CreateTileMap2i(this IEnumerable<SampledData2hi> templates)
        {
            return new TileMap2i(templates);
        }

        public static IEnumerable<SampledData2hi> CreateVariantsOfUmbra(this IEnumerable<SampledData2hi> templates)
        {
            return templates
                .SelectMany(CreateVariantsOfUmbra)
                .Distinct()
                .ToArray();
        }

        public static IEnumerable<SampledData2hi> CreateVariantsOfUmbra(this SampledData2hi tile)
        {
            for (Height1i x0y0 = Height1i.Default; x0y0 <= tile.x0y0.Abs; x0y0 = x0y0.Above)
            {
                for (Height1i x0y1 = Height1i.Default; x0y1 <= tile.x0y1.Abs; x0y1 = x0y1.Above)
                {
                    for (Height1i x1y0 = Height1i.Default; x1y0 <= tile.x1y0.Abs; x1y0 = x1y0.Above)
                    {
                        for (Height1i x1y1 = Height1i.Default; x1y1 <= tile.x1y1.Abs; x1y1 = x1y1.Above)
                        {
                            yield return new SampledData2hi
                              (
                                  (x0y0 * (tile.x0y0).Sign),
                                  (x0y1 * (tile.x0y1).Sign),
                                  (x1y0 * (tile.x1y0).Sign),
                                  (x1y1 * (tile.x1y1).Sign)
                              );
                        }
                    }
                }
            }
        }

        public static IEnumerable<SampledData2hi> ExpandRotations(this IEnumerable<SampledData2hi> templates)
        {
            return templates
                .SelectMany(template =>
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

        public static Range1hi RangeUnion(this IEnumerable<SampledData2hi> templates)
        {
            return templates
                .Select(o => o.Range)
                .Aggregate((Range1hi?)null, (a, b) => a?.UnionWith(b) ?? b) ?? Range1hi.Default;
        }

        public static IEnumerable<SampledData2hi> ScaleTemplates(this IEnumerable<SampledData2hi> templates, int scale)
        {
            for (int i = 1; i <= scale; i += 1)
            {
                foreach (var template in templates)
                {
                    yield return template * i;
                }
            }
        }
    }
}