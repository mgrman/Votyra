using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;


public class TerainGeneratorOptions
{
    public readonly TerainOptions TerainOptions;
    public readonly MeshOptions MeshOptions;
    
    public TerainGeneratorOptions(TerainGeneratorBehaviour terainGenerator)
    {
        this.TerainOptions = new TerainOptions(terainGenerator);
        this.MeshOptions = new MeshOptions(terainGenerator);
    }
    

    public bool IsChanged(TerainGeneratorOptions old)
    {
        return old == null ||
            TerainOptions.IsChanged(old.TerainOptions) ||
            MeshOptions.IsChanged(old.MeshOptions);
    }

    public bool IsBoundsChanged(TerainGeneratorOptions old)
    {
        return old==null ||
            TerainOptions.IsBoundsChanged(old.TerainOptions);
    }

    public bool IsValid
    {
        get
        {
            return TerainOptions.IsValid && MeshOptions.IsValid;
        }
    }
}
