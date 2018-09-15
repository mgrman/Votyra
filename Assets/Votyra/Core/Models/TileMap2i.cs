#define VERBOSE

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Votyra.Core.Models
{
    public class TileMap2i
    {
        private readonly IReadOnlyDictionary<SampledData2h, SampledData2h> _tileMap;

        public TileMap2i(IEnumerable<SampledData2h> templates)
        {
#if VERBOSE
            foreach (var template in templates)
            {
                Debug.Log($"{this.GetType().Name}-Template {template}");
            }
#endif
            Templates = templates.ToArray();
            ValueRange = Templates.RangeUnion();

            _tileMap = SampledData2h
                .GenerateAllValuesWithHoles(ValueRange)
                .ToDictionary(inputValue => inputValue, inputValue =>
                {
                    SampledData2h choosenTemplateTile = default(SampledData2h);
                    Height.Difference choosenTemplateTileDiff = Height.Difference.MaxValue;
                    foreach (SampledData2h tile in Templates)
                    {
                        var value = SampledData2h.Dif(tile, inputValue);
                        if (value < choosenTemplateTileDiff)
                        {
                            choosenTemplateTile = tile;
                            choosenTemplateTileDiff = value;
                        }
                    }
                    return choosenTemplateTile;
                });

#if VERBOSE
            foreach (var pair in _tileMap)
            {
                Debug.Log($"{this.GetType().Name} {pair.Key} => {pair.Value}");
            }
#endif
        }

        public IEnumerable<SampledData2h> Templates { get; }

        public Range1h ValueRange { get; }

        public SampledData2h GetTile(SampledData2h key)
        {
#if !VERBOSE
            return _tileMap[key];
#else
            SampledData2h value;
            if (_tileMap.TryGetValue(key, out value))
            {
                return value;
            }

            Debug.Log($"{this.GetType().Name} missing tile {key}");
            return key;
#endif
        }
    }
}