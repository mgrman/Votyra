using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;


public class TerainGenerator : MonoBehaviour
{
    public Vector2 CellSize = new Vector2(10, 10);
    public UI_Vector2i GroupCount = new UI_Vector2i(2, 2);
    public UI_Vector2i CellInGroupCount = new UI_Vector2i(10,10);

    public bool FlipTriangles=false;
    public bool ComputeOnAnotherThread;

    public Material Material = null;
    public MonoBehaviour Image = null;
    public MonoBehaviour MeshGenerator = null;
    public MonoBehaviour Sampler = null;

    private TerainGeneratorApplicator _terainGeneratorApplicator;

    void Start()
    {
        Update();
    }

    void Update()
    {
        TerainGeneratorOptions options = new TerainGeneratorOptions(this);

        //if (IsVisible())
        //{
            _terainGeneratorApplicator = _terainGeneratorApplicator ?? new TerainGeneratorApplicator();
            _terainGeneratorApplicator.UpdateMesh(options);
        //}
       // else
       // {
       //     Debug.Log("Skipping update!");
       // }
    }

    //private MeshFilter GetMeshFilter()
    //{
    //    MeshFilter meshFiler = TargetMesh ?? GetComponent<MeshFilter>() ?? this.gameObject.AddComponent<MeshFilter>();

    //    return meshFiler;
    //}

    //private Mesh GetMesh()
    //{
    //    MeshFilter meshFiler = GetMeshFilter();
    //    if (meshFiler.sharedMesh == null)
    //    {
    //        meshFiler.sharedMesh = new Mesh();
    //    }
    //    return meshFiler.sharedMesh;
    //}
    

    //private bool IsVisible()
    //{
    //    return Application.isEditor || GetMeshFilter().gameObject.GetComponent<MeshRenderer>().isVisible;
    //}
    
    private void OnDisable()
    {
        DisposeService();
    }

    private void OnDestroy()
    {
        DisposeService();
    }

    private void DisposeService()
    {
        if (_terainGeneratorApplicator != null)
        {
            _terainGeneratorApplicator.Dispose();
            _terainGeneratorApplicator = null;
        }
    }
}
