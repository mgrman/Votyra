using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Votyra.Core.Models
{
    public static class TileMap2iExtensions
    {
        public static TileMap2i CreateExpandedTileMap2i(this IEnumerable<SampledData2h> templates, int scaleFactor)
        {
            return templates
                .ScaleTemplates(scaleFactor)
                .CreateVariantsOfUmbra()
                .ExpandRotations()
                .CreateTileMap2i();
        }

        public static TileMap2i CreateTileMap2i(this IEnumerable<SampledData2h> templates)
        {
            return new TileMap2i(templates);
        }

        public static IEnumerable<SampledData2h> CreateVariantsOfUmbra(this IEnumerable<SampledData2h> templates)
        {
            return templates
                .SelectMany(CreateVariantsOfUmbra)
                .Distinct()
                .ToArray();
        }

        public static IEnumerable<SampledData2h> CreateVariantsOfUmbra(this SampledData2h tile)
        {
            for (Height x0y0 = Height.Default; x0y0 <= tile.x0y0.Abs; x0y0 = x0y0.Above)
            {
                for (Height x0y1 = Height.Default; x0y1 <= tile.x0y1.Abs; x0y1 = x0y1.Above)
                {
                    for (Height x1y0 = Height.Default; x1y0 <= tile.x1y0.Abs; x1y0 = x1y0.Above)
                    {
                        for (Height x1y1 = Height.Default; x1y1 <= tile.x1y1.Abs; x1y1 = x1y1.Above)
                        {
                            yield return new SampledData2h
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

        public static IEnumerable<SampledData2h> ExpandRotations(this IEnumerable<SampledData2h> templates)
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

        public static Range1h RangeUnion(this IEnumerable<SampledData2h> templates)
        {
            return templates
                .Select(o => o.Range)
                .Aggregate((Range1h?)null, (a, b) => a?.UnionWith(b) ?? b) ?? Range1h.Default;
        }

        public static IEnumerable<SampledData2h> ScaleTemplates(this IEnumerable<SampledData2h> templates, int scale)
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