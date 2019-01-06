#define VERBOSE

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Votyra.Core.Models
{
    public class TileMap2i
    {
        private readonly IReadOnlyDictionary<SampledData2i, SampledData2i> _tileMap;
        
        public TileMap2i(IEnumerable<SampledData2i> templates)
        {
#if VERBOSE
            foreach (var template in templates)
            {
                Debug.Log($"{this.GetType().Name}-Template {template}");
            }
#endif
            Templates = templates.ToArray();
            ValueRange = Templates.RangeUnion();

            _tileMap = SampledData2i
                .GenerateAllValuesWithHoles(ValueRange)
                .ToDictionary(inputValue => inputValue, inputValue =>
                {
                    SampledData2i choosenTemplateTile = default(SampledData2i);
                    float choosenTemplateTileDiff = int.MaxValue;
                    foreach (SampledData2i tile in Templates)
                    {
                        var value = SampledData2i.Dif(tile, inputValue);
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

        public IEnumerable<SampledData2i> Templates { get; }

        public Area1i ValueRange { get; }

        public SampledData2i GetTile(SampledData2i key)
        {
#if !VERBOSE
            return _tileMap[key];
#else
            SampledData2i value;
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