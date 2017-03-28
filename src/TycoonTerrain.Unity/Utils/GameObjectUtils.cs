using UnityEngine;

namespace TycoonTerrain.Unity.Utils
{
    public static class GameObjectUtils
    {
        public static void DestroyAllChildren(this GameObject gameObject)
        {
            gameObject.transform.DestroyAllChildren();
        }

        public static void DestroyAllChildren(this Transform transform)
        {
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(0);
                child.gameObject.Destroy();
            }
        }

        public static void Destroy(this GameObject gameObject)
        {
#if UNITY_EDITOR
        GameObject.DestroyImmediate(gameObject);
#else
            GameObject.Destroy(gameObject);
#endif
        }
    }
}