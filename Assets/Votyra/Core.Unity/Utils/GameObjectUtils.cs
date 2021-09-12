using UnityEngine;

namespace Votyra.Core.Unity.Utils
{
    public static class GameObjectUtils
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
                component = gameObject.AddComponent<T>();
            return component;
        }

        public static void AddComponentIfMissing<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
                gameObject.AddComponent<T>();
        }

        public static void Destroy(this GameObject gameObject)
        {
            if (gameObject == null)
                return;
#if UNITY_EDITOR
            Object.DestroyImmediate(gameObject);
#else
            GameObject.Destroy(gameObject);
#endif
        }

        public static void Destroy(this Object component)
        {
            if (component == null)
                return;
#if UNITY_EDITOR
            Object.DestroyImmediate(component);
#else
            GameObject.Destroy(component);
#endif
        }
    }
}