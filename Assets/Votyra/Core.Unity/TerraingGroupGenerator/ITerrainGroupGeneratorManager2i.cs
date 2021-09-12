using System;

namespace Votyra.Core.Unity.TerraingGroupGenerator
{
    public interface ITerrainGroupGeneratorManager2i : IDisposable
    {
        void Update(IFrameData2i context);
    }
}