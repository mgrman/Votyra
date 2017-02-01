using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class TerainOptions : IDisposable
{
    private const int MAX_CELL_COUNT = 60 * 60;

    public readonly Vector2 CellSize;
    public readonly Vector2i CellInGroupCount;
    public readonly Vector2 GroupSize;
    public readonly Bounds GroupBounds;
    public readonly Range2 RangeZ;
    
    public readonly IImage Image;
    public readonly IImageSampler ImageSampler;
    public readonly ITerainAlgorithm TerainAlgorithm;
    public readonly ITerainMesher TerainMesher;
    public readonly float Time;
    public readonly bool ComputeAsync;
    public readonly bool FlipTriangles;

    public List<Vector2i> GroupsToUpdate;

    public TerainOptions(TerainGeneratorBehaviour terainGenerator,IEnumerable<Vector2i> groupsToUpdate)
    {
        this.CellSize = terainGenerator.CellSize;
        this.CellInGroupCount = terainGenerator.CellInGroupCount;
        this.FlipTriangles = terainGenerator.FlipTriangles;

        if (this.CellInGroupCount.AreaSum > MAX_CELL_COUNT)
        {
            throw new InvalidOperationException("Too many cells in group! Max is 60x60");
        }
        
        this.Image = terainGenerator.Image as IImage;
        this.ImageSampler = terainGenerator.Sampler as IImageSampler;
        this.TerainAlgorithm = terainGenerator.MeshGenerator as ITerainAlgorithm;
        this.TerainMesher = terainGenerator.TerainMesher as ITerainMesher;
        this.Time = UnityEngine.Time.time;

#if UNITY_EDITOR

        if (Application.isPlaying)
        {
            this.ComputeAsync = terainGenerator.ComputeOnAnotherThread;
        }
        else
        {
            this.ComputeAsync = false;
        }
#else
        this.ComputeAsync = terainGenerator.ComputeOnAnotherThread;
#endif
   
        this.GroupSize = this.CellSize * this.CellInGroupCount;
        this.RangeZ = Image.RangeZ;
        this.GroupBounds = new Bounds((GroupSize / 2).ToVector3(RangeZ.Center), GroupSize.ToVector3(RangeZ.Size));

        this.GroupsToUpdate =Pool.Vector2iListPool.GetObject();
        this.GroupsToUpdate.Clear();
        this.GroupsToUpdate.AddRange(groupsToUpdate);
    }

    public bool IsChanged(TerainOptions old)
    {
        return old == null ||
            this.CellSize != old.CellSize ||
            this.CellInGroupCount != old.CellInGroupCount ||
            this.FlipTriangles != old.FlipTriangles ||
            this.Image != old.Image ||
            this.Image.IsChanged(this.Time) ||
            this.TerainAlgorithm != old.TerainAlgorithm ||
            this.ImageSampler != old.ImageSampler ||
            this.ComputeAsync != old.ComputeAsync ||
            this.RangeZ != old.RangeZ ||
            !this.GroupsToUpdate.SequenceEqual(old.GroupsToUpdate);
    }

    public bool IsBoundsChanged(TerainOptions old)
    {
        return old==null || 
            this.CellSize != old.CellSize ||
            this.CellInGroupCount != old.CellInGroupCount;
    }

    public bool IsValid
    {
        get
        {
            return this.TerainAlgorithm != null 
                && this.CellInGroupCount.Positive 
                && this.CellSize.Positive() 
                && this.Image != null 
                && this.ImageSampler != null
                && this.TerainMesher != null;
        }
    }

    public void Dispose()
    {
        Pool.Vector2iListPool.ReturnObject(GroupsToUpdate);
        GroupsToUpdate = null;
    }
}
