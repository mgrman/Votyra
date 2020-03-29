using Votyra.Core.Models;
using Votyra.Core.Utils;

namespace Votyra.Core.ImageSamplers
{
    public class ConstrainedDualImageSampler3b : IImageSampler3
    {
        public Vector3i WorldToImage(Vector3f pos) => new Vector3i((pos.X * 2).FloorToInt(), (pos.Y * 2).FloorToInt(), pos.Z.FloorToInt());

        public Vector3f ImageToWorld(Vector3i pos) => new Vector3f(pos.X / 2f, pos.Y / 2f, pos.Z);

        public Vector3i CellToX0Y0Z0(Vector3i pos) => new Vector3i(pos.X * 2 + 0, pos.Y * 2 + 0, pos.Z + 0);

        public Vector3i CellToX0Y0Z1(Vector3i pos) => new Vector3i(pos.X * 2 + 0, pos.Y * 2 + 0, pos.Z + 1);

        public Vector3i CellToX0Y1Z0(Vector3i pos) => new Vector3i(pos.X * 2 + 0, pos.Y * 2 + 1, pos.Z + 0);

        public Vector3i CellToX0Y1Z1(Vector3i pos) => new Vector3i(pos.X * 2 + 0, pos.Y * 2 + 1, pos.Z + 1);

        public Vector3i CellToX1Y0Z0(Vector3i pos) => new Vector3i(pos.X * 2 + 1, pos.Y * 2 + 0, pos.Z + 0);

        public Vector3i CellToX1Y0Z1(Vector3i pos) => new Vector3i(pos.X * 2 + 1, pos.Y * 2 + 0, pos.Z + 1);

        public Vector3i CellToX1Y1Z0(Vector3i pos) => new Vector3i(pos.X * 2 + 1, pos.Y * 2 + 1, pos.Z + 0);

        public Vector3i CellToX1Y1Z1(Vector3i pos) => new Vector3i(pos.X * 2 + 1, pos.Y * 2 + 1, pos.Z + 1);
    }
}
