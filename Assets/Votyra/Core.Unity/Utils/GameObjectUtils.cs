using UnityEngine;

namespace Votyra.Core.Utils
{
    public static class GameObjectUtils
    {
        public static GameObject NullIfDestroyed(this GameObject gameObject) => gameObject == null ? null : gameObject;

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

        public static void DestroyAllChildren(this GameObject gameObject)
        {
            gameObject.transform.DestroyAllChildren();
        }

        public static void DestroyAllChildren(this Transform transform)
        {
            var childCount = transform.childCount;
            for (var i = childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                child.gameObject.Destroy();
            }
        }

        public static void DestroyWithMeshes(this GameObject gameObject)
        {
            if (gameObject == null)
                return;
            var mesh = gameObject.GetComponent<MeshFilter>()
                .sharedMesh;
            mesh.Destroy();
            gameObject.Destroy();
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