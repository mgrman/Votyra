using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;


public class TerainGeneratorOptions
{
    private const int MAX_CELL_COUNT = 60 * 60;

    public readonly Vector2 CellSize;
    //public readonly Vector2i GroupCount;
    public readonly Vector2i CellInGroupCount;
    //public readonly Vector2i CellCount;
    public readonly Vector2 GroupSize;
    public readonly Bounds GroupBounds;
    public readonly Range2 RangeZ;
    //public readonly Vector2 Size;
    //public readonly Rect AoI;

    public readonly bool FlipTriangles;

    public readonly IImage Image;
    public readonly ISampler Sampler;
    public readonly IMeshGenerator MeshGenerator;
    public readonly Material Material;
    public readonly float Time;
    public readonly bool ComputeAsync;
    public readonly GameObject ParentContainer;
    public readonly bool DrawBounds;

    public readonly IList<Vector2i> GroupsToUpdate;

    //public TerainGeneratorOptions(Rect rect, Vector2 step, bool flip, IImage image, ISampler sampler, IMeshGenerator meshGen, float time)
    //{
    //    this.Rectangle = rect;
    //    this.Step = step;
    //    this.FlipTriangles = flip;
    //    this.Image = image;
    //    this.Sampler = sampler;
    //    this.MeshGenerator = meshGen;
    //    this.Time = time;
    //}

    //public TerainGeneratorOptions(TerainGeneratorTileManager terainGenerator,Vector2i cell)
    //{
    //    this.Rectangle = terainGenerator.Rectangle.GetRectangle(terainGenerator.TileCount, cell);
    //    this.Step = terainGenerator.Step;
    //    this.FlipTriangles = terainGenerator.FlipTriangles;
    //    this.OptimizeWalls = terainGenerator.OptimizeWalls;
    //    this.Image = terainGenerator.Image as IImage;
    //    this.Sampler = terainGenerator.Sampler as ISampler;
    //    this.MeshGenerator = terainGenerator.MeshGenerator as IMeshGenerator;
    //    this.Time = UnityEngine.Time.time;
    //}


    public TerainGeneratorOptions(TerainGenerator terainGenerator)
    {
        this.CellSize = terainGenerator.CellSize;
        //this.GroupCount = terainGenerator.GroupCount;
        this.CellInGroupCount = terainGenerator.CellInGroupCount;

        if (this.CellInGroupCount.AreaSum > MAX_CELL_COUNT)
        {
            throw new InvalidOperationException("Too many cells in group! Max is 60x60");
        }

        this.FlipTriangles = terainGenerator.FlipTriangles;

        this.Image = terainGenerator.Image as IImage;
        this.Sampler = terainGenerator.Sampler as ISampler;
        this.MeshGenerator = terainGenerator.MeshGenerator as IMeshGenerator;
        this.Time = UnityEngine.Time.time;
        this.Material = terainGenerator.Material;

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
        this.ParentContainer = terainGenerator.gameObject;
        this.DrawBounds = terainGenerator.DrawBounds;

        this.GroupSize = this.CellSize * this.CellInGroupCount;
        this.RangeZ = Image.RangeZ;
        this.GroupBounds = new Bounds((GroupSize / 2).ToVector3(RangeZ.Center), GroupSize.ToVector3(RangeZ.Size));
        //this.CellCount = this.GroupCount * this.CellInGroupCount;
       // this.Size = this.GroupSize * this.GroupCount;
        //this.AoI = new Rect(Vector2.zero, this.Size);

        //if(Time>1)
        //    this.GroupsToUpdate = Enumerable.Range(0, Math.Min(GroupCount.x, GroupCount.y)).Select(x => new Vector2i(x,x));
        //else
        this.GroupsToUpdate = VisibleGroups();

    }

    private IList<Vector2i> VisibleGroups()
    {
        var cam = Camera.main;

        var mat = cam.projectionMatrix * cam.worldToCameraMatrix* ParentContainer.transform.localToWorldMatrix;

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


    public bool IsChanged(TerainGeneratorOptions old)
    {
        //var size = this.CellSize * this.CellInGroupCount * this.GroupCount;
        //var rect = new Rect(Vector2.zero, size);

        return old == null || 
            this.CellSize != old.CellSize ||
            this.CellInGroupCount != old.CellInGroupCount ||
            this.FlipTriangles != old.FlipTriangles ||
            this.Image != old.Image ||
            this.Image.IsChanged(this.Time) ||
            this.Sampler != old.Sampler ||
            this.MeshGenerator != old.MeshGenerator ||
            this.Material != old.Material ||
            this.ComputeAsync != old.ComputeAsync ||
            this.ParentContainer != old.ParentContainer ||
            this.RangeZ != old.RangeZ;
    }

    public bool IsBoundsChanged(TerainGeneratorOptions old)
    {
        return old==null || 
            this.CellSize != old.CellSize ||
            this.CellInGroupCount != old.CellInGroupCount;
    }
    public bool IsValid
    {
        get
        {
            return this.CellInGroupCount.Positive && this.CellSize.Positive() && this.Image != null && this.Sampler != null && this.MeshGenerator != null && this.ParentContainer != null;
        }
    }
}
