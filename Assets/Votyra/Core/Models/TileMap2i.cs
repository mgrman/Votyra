#define VERBOSE

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Votyra.Core.Models
{
    public class TileMap2i
    {
        private readonly IReadOnlyDictionary<SampledData2hi, SampledData2hi> _tileMap;

        public TileMap2i(IEnumerable<SampledData2hi> templates)
        {
#if VERBOSE
            foreach (var template in templates)
            {
                Debug.Log($"{this.GetType().Name}-Template {template}");
            }
#endif
            Templates = templates.ToArray();
            ValueRange = Templates.RangeUnion();

            _tileMap = SampledData2hi
                .GenerateAllValuesWithHoles(ValueRange)
                .ToDictionary(inputValue => inputValue, inputValue =>
                {
                    SampledData2hi choosenTemplateTile = default(SampledData2hi);
                    Height1i.Difference choosenTemplateTileDiff = Height1i.Difference.MaxValue;
                    foreach (SampledData2hi tile in Templates)
                    {
                        var value = SampledData2hi.Dif(tile, inputValue);
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

        public IEnumerable<SampledData2hi> Templates { get; }

        public Range1hi ValueRange { get; }

        public SampledData2hi GetTile(SampledData2hi key)
        {
#if !VERBOSE
            return _tileMap[key];
#else
            SampledData2hi value;
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