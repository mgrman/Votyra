namespace Votyra.Core.Models
{
    public enum MaskValues : byte
    {
        Terrain = 0,
        Hole = 1
    }

    public static class MaskValuesExtensions
    {
        public static bool IsHole(this MaskValues value)
        {
            return (value & MaskValues.Hole) != 0;
        }

        public static bool IsNotHole(this MaskValues value)
        {
            return (value & MaskValues.Hole) == 0;
        }
    }
}