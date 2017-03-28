using TycoonTerrain.Unity.Models;

namespace TycoonTerrain.Unity.Utils
{
    public static class UnityUtils
    {
        public static Common.Models.Vector2i ToDomain(this UI_Vector2i vector)
        {
            return new Common.Models.Vector2i(vector.x, vector.y);
        }

        public static UnityEngine.Vector2 ToUnity(this Common.Models.Vector2 vector)
        {
            return new UnityEngine.Vector2(vector.x, vector.y);
        }

        public static UnityEngine.Vector3 ToUnity(this Common.Models.Vector3 vector)
        {
            return new UnityEngine.Vector3(vector.x, vector.y, vector.z);
        }
        public static Common.Models.Vector3 ToDomain(this UnityEngine.Vector3 vector)
        {
            return new Common.Models.Vector3(vector.x, vector.y, vector.z);
        }

        public static UnityEngine.Bounds ToUnity(this Common.Models.Bounds bounds)
        {
            return new UnityEngine.Bounds(bounds.center.ToUnity(), bounds.size.ToUnity());
        }

        public static Common.Models.Bounds ToDomain(this UnityEngine.Bounds bounds)
        {
            return new Common.Models.Bounds(bounds.center.ToDomain(), bounds.size.ToDomain());
        }
    }
}