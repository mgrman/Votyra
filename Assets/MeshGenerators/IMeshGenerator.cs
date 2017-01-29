using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface IMeshGenerator
{
    bool RequiresWalls { get; }

    HeightData Process(HeightData sampleData);
}
