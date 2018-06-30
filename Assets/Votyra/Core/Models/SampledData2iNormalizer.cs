
namespace Votyra.Core.Models
{
    public struct SampledData2iNormalizer
    {
        public readonly SampledData2i NormalizedValue;

        public readonly int? ZeroHeight;

        public SampledData2iNormalizer(SampledData2i sampleData, Range1i range)
        {
            ZeroHeight = sampleData.Max - range.Min;
            NormalizedValue = (sampleData - ZeroHeight).ClipMin(-range.Size);
        }

        public SampledData2i Denormalize(SampledData2i sampleData)
        {
            return sampleData + ZeroHeight;
        }
    }
}