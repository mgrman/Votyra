using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;


public class TerainOptions
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

    public readonly IList<Vector2i> GroupsToUpdate;

    public TerainOptions(TerainGeneratorBehaviour terainGenerator)
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
  
        this.GroupsToUpdate = VisibleGroups(terainGenerator.gameObject);
    }

    private IList<Vector2i> VisibleGroups(GameObject parentContainer)
    {
        var cam = Camera.main;

        var mat = cam.projectionMatrix * cam.worldToCameraMatrix* parentContainer.transform.localToWorldMatrix;

        var planes = GeometryUtility.CalculateFrustumPlanes(mat);
        
        var bounds_center = GroupBounds.center;
        var bounds_size = GroupBounds.size; 
        var groupSize_x = GroupSize.x;
        var groupSize_y = GroupSize.y;
        var res = new List<Vector2i>();
        

        for (int group_x = -5; group_x < 5; group_x++)
        {
            for (int group_y = -5; group_y < 5; group_y++)
            {
                var bounds = new Bounds( new Vector3
                    (
                        bounds_center.x + group_x * groupSize_x,
                        bounds_center.y + group_y * groupSize_y,
                        bounds_center.z
                    ),bounds_size);

               // bounds = ParentContainer.transform.TransformBounds(bounds);


                if (GeometryUtility.TestPlanesAABB(planes, bounds))
                {
                    res.Add( new Vector2i(group_x, group_y));
                }
            }
        }
        return res;
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
            this.RangeZ != old.RangeZ;
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
}
