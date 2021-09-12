using System;

namespace Votyra.Core
{
    public interface ITerrainGroupGeneratorManager2i : IDisposable
    {
        void Update(IFrameData2i context);
    }
}