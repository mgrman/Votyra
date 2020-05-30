using System;
using Votyra.Core.Models;
using Votyra.Core.Pooling;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core
{
    public struct GroupUpdateData : IDisposable
    {
        public GroupUpdateData(Vector2i group, ArcResource<IFrameData2i> context, ITerrainMesh2f mesh, bool forceUpdate)
        {
            this.Group = group;
            this.Context = context;
            this.Mesh = mesh;
            this.ForceUpdate = forceUpdate;
        }

        public Vector2i Group { get; }

        public ArcResource<IFrameData2i> Context { get; }

        public ITerrainMesh2f Mesh { get; }

        public bool ForceUpdate { get; }

        public void Dispose()
        {
            this.Context?.Dispose();
        }
    }
}
