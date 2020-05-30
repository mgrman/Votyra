#define VERBOSE

using System.Collections.Generic;
using System.Linq;
using Votyra.Core.Logging;

namespace Votyra.Core.Models
{
    public class TileMap2I
    {
        private readonly IThreadSafeLogger logger;
        private readonly IReadOnlyDictionary<SampledData2I, SampledData2I> tileMap;

        public TileMap2I(IEnumerable<SampledData2I> templates, IThreadSafeLogger logger)
        {
            this.logger = logger;
            // #if VERBOSE
            //             foreach (var template in templates)
            //             {
            //                 _logger.LogMessage($"{GetType().Name}-Template {template}");
            //             }
            // #endif
            this.Templates = templates.ToArray();
            this.ValueRange = this.Templates.RangeUnion();

            this.tileMap = SampledData2I.GenerateAllValuesWithHoles(this.ValueRange)
                .ToDictionary(inputValue => inputValue,
                    inputValue =>
                    {
                        var choosenTemplateTile = default(SampledData2I);
                        float choosenTemplateTileDiff = int.MaxValue;
                        foreach (var tile in this.Templates)
                        {
                            var value = SampledData2I.Dif(tile, inputValue);
                            if (value < choosenTemplateTileDiff)
                            {
                                choosenTemplateTile = tile;
                                choosenTemplateTileDiff = value;
                            }
                        }

                        return choosenTemplateTile;
                    });

#if VERBOSE
            foreach (var pair in this.tileMap)
            {
                this.logger.LogMessage($"{this.GetType().Name} {pair.Key} => {pair.Value}");
            }
#endif
        }

        public IEnumerable<SampledData2I> Templates { get; }

        public Area1i ValueRange { get; }

        public SampledData2I GetTile(SampledData2I key)
        {
#if !VERBOSE
            return _tileMap[key];
#else
            SampledData2I value;
            if (this.tileMap.TryGetValue(key, out value))
            {
                return value;
            }

            this.logger.LogMessage($"{this.GetType().Name} missing tile {key}");
            return key;
#endif
        }
    }
}
