using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public interface ITerainGenerator 
{
    IEnumerable<ITriangleMesh> Generate(TerainOptions options);
}
