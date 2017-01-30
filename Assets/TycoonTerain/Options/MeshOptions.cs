using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;


public class MeshOptions
{
    
    public readonly Material Material;
    public readonly GameObject ParentContainer;
    public readonly bool DrawBounds;
    
    public MeshOptions(TerainGeneratorBehaviour terainGenerator)
    {
        
        this.Material = terainGenerator.Material;

        this.ParentContainer = terainGenerator.gameObject;
        this.DrawBounds = terainGenerator.DrawBounds;
    }
    
    public bool IsChanged(MeshOptions old)
    {
        return old == null ||
            this.Material != old.Material ||
            this.ParentContainer != old.ParentContainer;
    }
    
    public bool IsValid
    {
        get
        {
            return  this.ParentContainer != null;
        }
    }
}
