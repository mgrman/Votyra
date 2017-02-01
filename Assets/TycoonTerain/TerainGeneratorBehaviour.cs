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


    private IGroupSelector _groupsSelector;
    private ITerainGenerator _terainGenerator;
    private IMeshUpdater _meshUpdater;

    public static Thread UnityThread { get; private set; }

    void Start()
    {
        UnityThread = Thread.CurrentThread;
        this.gameObject.DestroyAllChildren();
    }

    void Update()
    {
        Profiler.BeginSample("Updating cached services");
        UpdateCachedServices();
        Profiler.EndSample();

        Profiler.BeginSample("Creating visible groups");
        var groupVisibilityOptions = new GroupVisibilityOptions(this);
        var groupsToUpdate = _groupsSelector.GetGroupsToUpdate(groupVisibilityOptions);
        Profiler.EndSample();
        
        Profiler.BeginSample("Sampling mesh");
        TerainOptions terainOptions = new TerainOptions(this, groupsToUpdate);
        var results=_terainGenerator.Generate(terainOptions);
        Profiler.EndSample();

        Profiler.BeginSample("Applying mesh");
        MeshOptions meshOptions = new MeshOptions(this);
        _meshUpdater.UpdateMesh(meshOptions, results);
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
        ObjectUtils.UpdateType<GroupsByCameraVisibilitySelector, IGroupSelector>(ref _groupsSelector);
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
