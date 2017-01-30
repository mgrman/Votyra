using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

[ExecuteInEditMode]
public class TerainGeneratorBehaviour : MonoBehaviour
{
    public Vector2 CellSize = new Vector2(10, 10);
    public UI_Vector2i CellInGroupCount = new UI_Vector2i(10, 10);

    public bool FlipTriangles = false;
    public bool ComputeOnAnotherThread;
    public bool DrawBounds;

    public Material Material = null;
    public MonoBehaviour Image = null;
    public MonoBehaviour MeshGenerator = null;
    public MonoBehaviour TerainMesher = null;
    public MonoBehaviour Sampler = null;


    private ITerainGenerator _terainGenerator;
    private IMeshUpdater _meshUpdater;

    void Start()
    {
        this.gameObject.DestroyAllChildren();
    }

    void Update()
    {
        Profiler.BeginSample("Updating cached services");
        UpdateCachedServices();
        Profiler.EndSample();

        Profiler.BeginSample("Creating options");
        TerainGeneratorOptions options = new TerainGeneratorOptions(this);
        Profiler.EndSample();
        
        Profiler.BeginSample("Sampling mesh");
        var results=_terainGenerator.Generate(options.TerainOptions);
        Profiler.EndSample();

        Profiler.BeginSample("Applying mesh");
        _meshUpdater.UpdateMesh(options.MeshOptions, results);
        Profiler.EndSample();
    }

    private void UpdateCachedServices()
    {
        if (this.ComputeOnAnotherThread)
        {
            ObjectUtils.UpdateType<AsyncTerainGenerator<TerainGenerator>,ITerainGenerator>(ref _terainGenerator);
        }
        else
        {
            ObjectUtils.UpdateType<TerainGenerator, ITerainGenerator>(ref _terainGenerator);
        }

        ObjectUtils.UpdateType<TerainMeshUpdater, IMeshUpdater>(ref _meshUpdater);
    }

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
        if (_terainGenerator is IDisposable)
        {
            (_terainGenerator as IDisposable).Dispose();
        }
        _terainGenerator = null;

        if (_meshUpdater is IDisposable)
        {
            (_meshUpdater as IDisposable).Dispose();
        }
        _meshUpdater = null;
    }
}
