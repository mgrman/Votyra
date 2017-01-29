using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

[ExecuteInEditMode]
public class TerainGenerator : MonoBehaviour
{
    public Vector2 CellSize = new Vector2(10, 10);
    public UI_Vector2i CellInGroupCount = new UI_Vector2i(10, 10);

    public bool FlipTriangles = false;
    public bool ComputeOnAnotherThread;
    public bool DrawBounds;

    public Material Material = null;
    public MonoBehaviour Image = null;
    public MonoBehaviour MeshGenerator = null;
    public MonoBehaviour Sampler = null;


    private ITerainGeneratorService _terainGeneratorService;
    private ITerainGeneratorApplicator _terainGeneratorApplicator;

    void Start()
    {
        this.gameObject.DestroyAllChildren();
    }

    void Update()
    {

        Profiler.BeginSample("Creating options");
        TerainGeneratorOptions options = new TerainGeneratorOptions(this);
        Profiler.EndSample();

        //if (IsVisible())
        //{


        //Debug.Log("Updating mesh!");

        Profiler.BeginSample("Sampling mesh");
        var service = GetTerainGeneratorService(options);
        IDictionary<Vector2i, ITriangleMesh> triangleMeshes = service.Sample(options);
        Profiler.EndSample();

        Profiler.BeginSample("Applying mesh");
        var terainGeneratorApplicator = GetTerainGeneratorApplicator();
        terainGeneratorApplicator.UpdateMesh(options, triangleMeshes);
        Profiler.EndSample();

        //}
        // else
        // {
        //     Debug.Log("Skipping update!");
        // }
    }



    private ITerainGeneratorService GetTerainGeneratorService(TerainGeneratorOptions options)
    {
        if (options.ComputeAsync)
        {
            if (_terainGeneratorService == null)
            {
                //Debug.Log("Creating new AsyncTerainGeneratorService");
                _terainGeneratorService = new AsyncTerainGeneratorService();
            }
            else if (!(_terainGeneratorService is AsyncTerainGeneratorService))
            {
                //Debug.Log("Overriding with AsyncTerainGeneratorService");
                _terainGeneratorService.Dispose();
                _terainGeneratorService = new AsyncTerainGeneratorService();
            }
        }
        else
        {
            if (_terainGeneratorService == null)
            {
                //Debug.Log("Creating new TerainGeneratorService");
                _terainGeneratorService = new TerainGeneratorService();
            }
            else if (!(_terainGeneratorService is TerainGeneratorService))
            {
                //Debug.Log("Overriding with TerainGeneratorService");
                _terainGeneratorService.Dispose();
                _terainGeneratorService = new TerainGeneratorService();
            }
        }
        return _terainGeneratorService;
    }

    private ITerainGeneratorApplicator GetTerainGeneratorApplicator()
    {
        _terainGeneratorApplicator= _terainGeneratorApplicator ?? new TerainGeneratorApplicator();
        return _terainGeneratorApplicator;
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

        if (_terainGeneratorService != null)
        {
            _terainGeneratorService.Dispose();
            _terainGeneratorService = null;
        }
    }
}
