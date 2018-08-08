using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Votyra.Core.Models
{
    public static class TileMap2iExtensions
    {
        public static TileMap2i CreateExpandedTileMap2i(this IEnumerable<SampledData2i> templates, int scaleFactor)
        {
            return templates
                .ScaleTemplates(scaleFactor)
                .CreateVariantsOfUmbra()
                .ExpandRotations()
                .CreateTileMap2i();
        }

        public static TileMap2i CreateTileMap2i(this IEnumerable<SampledData2i> templates)
        {
            return new TileMap2i(templates);
        }

        public static IEnumerable<SampledData2i> CreateVariantsOfUmbra(this IEnumerable<SampledData2i> templates)
        {
            return templates
                .SelectMany(CreateVariantsOfUmbra)
                .Distinct()
                .ToArray();
        }

        public static IEnumerable<SampledData2i> CreateVariantsOfUmbra(this SampledData2i tile)
        {
            for (int x0y0 = 0; x0y0 <= Math.Abs(tile.x0y0 ?? 0); x0y0++)
            {
                for (int x0y1 = 0; x0y1 <= Math.Abs(tile.x0y1 ?? 0); x0y1++)
                {
                    for (int x1y0 = 0; x1y0 <= Math.Abs(tile.x1y0 ?? 0); x1y0++)
                    {
                        for (int x1y1 = 0; x1y1 <= Math.Abs(tile.x1y1 ?? 0); x1y1++)
                        {
                            yield return new SampledData2i
                              (
                                  (x0y0 * Math.Sign(tile.x0y0 ?? 0)).ReturnHoleIfHole(tile.x0y0),
                                  (x0y1 * Math.Sign(tile.x0y1 ?? 0)).ReturnHoleIfHole(tile.x0y1),
                                  (x1y0 * Math.Sign(tile.x1y0 ?? 0)).ReturnHoleIfHole(tile.x1y0),
                                  (x1y1 * Math.Sign(tile.x1y1 ?? 0)).ReturnHoleIfHole(tile.x1y1)
                              );
                        }
                    }
                }
            }
        }

        public static IEnumerable<SampledData2i> ExpandRotations(this IEnumerable<SampledData2i> templates)
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

        public static Range1i RangeUnion(this IEnumerable<SampledData2i> templates)
        {
            return templates
                .Select(o => o.Range)
                .Aggregate((Range1i?)null, (a, b) => a?.UnionWith(b) ?? b) ?? Range1i.Zero;
        }

        public static int? ReturnHoleIfHole(this int val, int? holeTest)
        {
            return holeTest.IsHole() ? holeTest : val;
        }

        public static IEnumerable<SampledData2i> ScaleTemplates(this IEnumerable<SampledData2i> templates, int scale)
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