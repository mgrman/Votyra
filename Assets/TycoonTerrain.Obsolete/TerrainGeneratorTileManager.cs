using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

public class TerainGeneratorTileManager : MonoBehaviour
{
    //public Rect Rectangle = new Rect(-10, -10, 20, 20);
    //public Vector2 Step = new Vector2(1, 1);
    //public int TileCountX = 2;
    //public int TileCountY = 2;
    //public Vector2i TileCount
    //{
    //    get { return new Vector2i(TileCountX, TileCountY); }
    //}

    //public bool FlipTriangles;
    //public bool OptimizeWalls;
    //public bool ComputeOnAnotherThread;

    //public Material Material;
    //public MonoBehaviour Image;
    //public MonoBehaviour MeshGenerator;
    //public MonoBehaviour Sampler;

    //private ITerainGeneratorService _service;
    
    //void Start()
    //{
    //}

    //void Update()
    //{




    //    List<GameObject> usedNames = new List<GameObject>();
    //    for (int ix = 0; ix < TileCountX; ix++)
    //    {
    //        for (int iy = 0; iy < TileCountY; iy++)
    //        {
    //           var name= CreateTileIfNotExisting(ix,iy);
    //            usedNames.Add(name);
    //        }
    //    }

    //    var toDelete=Enumerable.Range(0, this.transform.childCount).Select(i => this.transform.GetChild(i).gameObject)
    //        .Except(usedNames)
    //        .ToArray();

    //    foreach(var obj in toDelete)
    //    {
    //        DestroyImmediate(obj);
    //    }
    //}

    //private GameObject CreateTileIfNotExisting(int ix, int iy)
    //{
    //    string name = string.Format("tile_{0}_{1}", ix, iy);
    //    GameObject tile;

    //    var tileTransform = this.transform.FindChild(name);
    //    if (tileTransform == null)
    //    {
    //        tile = new GameObject(name);
    //        tile.transform.SetParent(this.transform,false);
    //    }
    //    else
    //    {
    //        tile = tileTransform.gameObject;
    //    }

    //    var meshFilter = tile.GetComponent<MeshFilter>();
    //    if (meshFilter == null)
    //    {
    //        meshFilter = tile.AddComponent<MeshFilter>();
    //    }
    //    var meshRenderer = tile.GetComponent<MeshRenderer>();
    //    if (meshRenderer == null)
    //    {
    //        meshRenderer= tile.AddComponent<MeshRenderer>();
    //    }
    //    meshRenderer.material = Material;

    //    var terainGenerator = tile.GetComponent<TerainGenerator>();
    //    if (terainGenerator == null)
    //    {
    //        terainGenerator= tile.AddComponent<TerainGenerator>();
    //    }
        
    //    terainGenerator.Rectangle = GetRectangle(ix, iy);
    //    terainGenerator.Step = this.Step;
    //    terainGenerator.FlipTriangles = this.FlipTriangles;
    //    terainGenerator.OptimizeWalls = this.OptimizeWalls;
    //    terainGenerator.ComputeOnAnotherThread = this.ComputeOnAnotherThread;
    //    terainGenerator.ThreadSleepFor = this.ThreadSleepFor;

    //    terainGenerator.Image = this.Image;
    //    terainGenerator.MeshGenerator = this.MeshGenerator;
    //    terainGenerator.Sampler = this.Sampler;

    //    return tile;
    //}
}
