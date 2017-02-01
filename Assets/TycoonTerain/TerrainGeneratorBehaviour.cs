using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

[ExecuteInEditMode]
public class TerrainGeneratorBehaviour : MonoBehaviour
{
    public UI_Vector2i CellInGroupCount = new UI_Vector2i(10, 10);

    public bool FlipTriangles = false;
    public bool ComputeOnAnotherThread;
    public bool DrawBounds;

    public Material Material = null;
    public MonoBehaviour Image = null;
    public MonoBehaviour MeshGenerator = null;
    public MonoBehaviour TerrainMesher = null;
    public MonoBehaviour Sampler = null;


    private IGroupSelector _groupsSelector;
    private ITerrainGenerator _terrainGenerator;
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
        TerrainOptions terrainOptions = new TerrainOptions(this, groupsToUpdate);
        var results=_terrainGenerator.Generate(terrainOptions);
        Profiler.EndSample();

        Profiler.BeginSample("Applying mesh");
        MeshOptions meshOptions = new MeshOptions(this);
        _meshUpdater.UpdateMesh(meshOptions, results);
        Profiler.EndSample();
    }

    private void UpdateCachedServices()
    {

        bool computeOnAnotherThread = this.ComputeOnAnotherThread;
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            computeOnAnotherThread = false;
        }
#endif

        if (computeOnAnotherThread)
        {
            ObjectUtils.UpdateType<AsyncTerrainGenerator<TerrainGenerator>,ITerrainGenerator>(ref _terrainGenerator);
        }
        else
        {
            ObjectUtils.UpdateType<TerrainGenerator, ITerrainGenerator>(ref _terrainGenerator);
        }

        ObjectUtils.UpdateType<TerrainMeshUpdater, IMeshUpdater>(ref _meshUpdater);
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
        if (_terrainGenerator is IDisposable)
        {
            (_terrainGenerator as IDisposable).Dispose();
        }
        _terrainGenerator = null;

        if (_meshUpdater is IDisposable)
        {
            (_meshUpdater as IDisposable).Dispose();
        }
        _meshUpdater = null;
    }
}
