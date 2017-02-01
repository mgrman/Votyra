﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;


public class MeshOptions : IDisposable
{
    
    public readonly Material Material;
    public readonly GameObject ParentContainer;
    public readonly bool DrawBounds;
    
    public MeshOptions(TerrainGeneratorBehaviour terrainGenerator)
    {
        
        this.Material = terrainGenerator.Material;

        this.ParentContainer = terrainGenerator.gameObject;
        this.DrawBounds = terrainGenerator.DrawBounds;
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

    public void Dispose()
    {

    }
}
