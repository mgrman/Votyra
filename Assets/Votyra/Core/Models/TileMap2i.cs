using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Votyra.Core.Models
{
    public class TileMap2i
    {
        private readonly IReadOnlyDictionary<SampledData2i, SampledData2i> _tileMap;

        public TileMap2i(SampledData2i[] templates)
            : this(templates, GetRangeFromTemplates(templates), true)
        {
        }

        public TileMap2i(SampledData2i[] templates, Range1i valueRange, bool expandRotations)
        {
            if (expandRotations)
            {
                templates = templates
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

            _tileMap = SampledData2i
                .GenerateAllValues(new Range1i(valueRange.Min, valueRange.Max), true)
                .ToDictionary(inputValue => inputValue, inputValue =>
                {
                    SampledData2i choosenTemplateTile = default(SampledData2i);
                    float choosenTemplateTileDiff = float.MaxValue;
                    for (int it = 0; it < templates.Length; it++)
                    {
                        SampledData2i tile = templates[it];
                        var value = SampledData2i.Dif(tile, inputValue);
                        if (value < choosenTemplateTileDiff)
                        {
                            choosenTemplateTile = tile;
                            choosenTemplateTileDiff = value;
                        }
                    }
                    return choosenTemplateTile.SetHolesUsing(inputValue);
                });
        }

        public SampledData2i GetTile(SampledData2i key) => _tileMap[key];

        private static Range1i GetRangeFromTemplates(SampledData2i[] templates)
        {
            return templates
                .Select(o => o.Range)
                .Aggregate((Range1i?)null,
                (a, b) => a?.UnionWith(b) ?? b) ?? Range1i.Zero;
        }
    }
}