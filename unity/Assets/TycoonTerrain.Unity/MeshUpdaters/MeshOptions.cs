using System;
using TycoonTerrain.Common.Models;
using UnityEngine;

namespace TycoonTerrain.Unity.MeshUpdaters
{
    public class MeshOptions : IDisposable
    {
        public readonly Material Material;
        public readonly GameObject ParentContainer;
        public readonly bool DrawBounds;
        public readonly Func<GameObject> GameObjectFactory;

        public MeshOptions(Material material, GameObject parentContainer, bool drawBounds, Func<GameObject> gameObjectFactory)
        {
            this.Material = material;

            this.ParentContainer = parentContainer;
            this.DrawBounds = drawBounds;
            this.GameObjectFactory = gameObjectFactory;
        }

        public bool IsChanged(MeshOptions old)
        {
            return old == null ||
                this.Material != old.Material ||
                this.ParentContainer != old.ParentContainer ||
                this.GameObjectFactory != old.GameObjectFactory;
        }

        public bool IsValid
        {
            get
            {
                return this.ParentContainer != null;
            }
        }

        public void Dispose()
        {
        }
    }
}