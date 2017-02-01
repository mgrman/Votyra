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
    
    public readonly Vector2i CellInGroupCount;
    public readonly Bounds GroupBounds;
    public readonly Range2i RangeZ;
    
    public readonly IImage2i Image;
    public readonly IImageSampler ImageSampler;
    public readonly ITerainAlgorithm TerainAlgorithm;
    public readonly ITerainMesher TerainMesher;
    public readonly float Time;
    public readonly bool ComputeAsync;
    public readonly bool FlipTriangles;

    public IList<Vector2i> GroupsToUpdate;

    public TerainOptions(TerainGeneratorBehaviour terainGenerator,IList<Vector2i> groupsToUpdate)
    {
        this.CellInGroupCount = terainGenerator.CellInGroupCount;
        this.FlipTriangles = terainGenerator.FlipTriangles;

        if (this.CellInGroupCount.AreaSum > MAX_CELL_COUNT)
        {
            throw new InvalidOperationException("Too many cells in group! Max is 60x60");
        }
        
        this.Image = new RoundImage( terainGenerator.Image as IImage2);
        this.ImageSampler = terainGenerator.Sampler as IImageSampler;
        this.TerainAlgorithm = terainGenerator.MeshGenerator as ITerainAlgorithm;
        this.TerainMesher = terainGenerator.TerainMesher as ITerainMesher;
        this.Time = UnityEngine.Time.time;


   
        this.RangeZ = Image.RangeZ;
        this.GroupBounds = new Bounds(new Vector3(CellInGroupCount.x / 2.0f, CellInGroupCount.y / 2.0f,RangeZ.Center), new Vector3(CellInGroupCount.x, CellInGroupCount.y , RangeZ.Size) );

        this.GroupsToUpdate = groupsToUpdate;
    }

    public bool IsChanged(TerainOptions old)
    {
        return old == null ||
            this.CellInGroupCount != old.CellInGroupCount ||
            this.FlipTriangles != old.FlipTriangles ||
            this.Image != old.Image ||
            this.Image.IsAnimated ||
            this.TerainAlgorithm != old.TerainAlgorithm ||
            this.ImageSampler != old.ImageSampler ||
            this.RangeZ != old.RangeZ ||
            !this.GroupsToUpdate.SequenceEqual(old.GroupsToUpdate);
    }

    public bool IsBoundsChanged(TerainOptions old)
    {
        return old==null || 
            this.CellInGroupCount != old.CellInGroupCount;
    }

    public bool IsValid
    {
        get
        {
            return this.TerainAlgorithm != null 
                && this.CellInGroupCount.Positive 
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
