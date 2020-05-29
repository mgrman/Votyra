#define VERBOSE

using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Logging;

namespace Votyra.Core.Models
{
    public class TileMap2i
    {
        private readonly IThreadSafeLogger _logger;
        private readonly IReadOnlyDictionary<SampledData2i, SampledData2i> _tileMap;

        public TileMap2i(IEnumerable<SampledData2i> templates, IThreadSafeLogger logger)
        {
            this._logger = logger;
            // #if VERBOSE
            //             foreach (var template in templates)
            //             {
            //                 _logger.LogMessage($"{GetType().Name}-Template {template}");
            //             }
            // #endif
            this.Templates = templates.ToArray();
            this.ValueRange = this.Templates.RangeUnion();

            this._tileMap = SampledData2i.GenerateAllValuesWithHoles(this.ValueRange)
                .ToDictionary(inputValue => inputValue,
                    inputValue =>
                    {
                        var choosenTemplateTile = default(SampledData2i);
                        float choosenTemplateTileDiff = int.MaxValue;
                        foreach (var tile in this.Templates)
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
            foreach (var pair in this._tileMap)
            {
                this._logger.LogMessage($"{this.GetType().Name} {pair.Key} => {pair.Value}");
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
            if (this._tileMap.TryGetValue(key, out value))
            {
                return value;
            }

            this._logger.LogMessage($"{this.GetType().Name} missing tile {key}");
            return key;
#endif
        }
    }
}
