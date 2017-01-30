using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;


public class GroupVisibilityOptions
{
    
    public readonly GameObject ParentContainer;
    public readonly Vector2 GroupSize;
    public readonly Camera Camera;
    public readonly Bounds GroupBounds;
    public readonly Range2 RangeZ;


    private readonly Vector2 CellSize;
    private readonly Vector2i CellInGroupCount;
    private readonly IImage Image;

    public GroupVisibilityOptions(TerainGeneratorBehaviour terainGenerator)
    {
        this.Camera = Camera.main;
        this.ParentContainer = terainGenerator.gameObject;
        this.Image = terainGenerator.Image as IImage;
        this.RangeZ = Image.RangeZ;
        this.CellSize = terainGenerator.CellSize;
        this.CellInGroupCount = terainGenerator.CellInGroupCount;

        this.GroupSize = this.CellSize * this.CellInGroupCount;
        this.GroupBounds = new Bounds((this.GroupSize / 2).ToVector3(this.RangeZ.Center), this.GroupSize.ToVector3(this.RangeZ.Size));
    }
    
    public bool IsChanged(GroupVisibilityOptions old)
    {
        //TODO
        //not complete check!
        return old == null ||
            this.ParentContainer != old.ParentContainer ||
            this.Camera != old.Camera ||
            this.GroupSize != old.GroupSize;
    }
    
    public bool IsValid
    {
        get
        {
            return  this.ParentContainer != null;
        }
    }
}
