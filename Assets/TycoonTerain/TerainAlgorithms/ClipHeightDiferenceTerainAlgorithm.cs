using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ClipHeightDiferenceTerainAlgorithm : MonoBehaviour, ITerainAlgorithm
{
    [Range(0.0F, 5.0f)]
    public float HeightDifferenceAbs = 1;
    [Range(0.0F, 1.0F)]
    public float HeightDifferenceRel = 1;
    public HeightClippingTypes ClippingType = HeightClippingTypes.ClipMin;

    public bool RequiresWalls { get { return true; } }

    public HeightData Process(HeightData sampleData)
    {
        return Process(sampleData, HeightDifferenceAbs, HeightDifferenceRel, ClippingType);
    }

    public static HeightData Process(HeightData heightData, float heightDifferenceAbs,float heightDifferenceRel, HeightClippingTypes clippingType)
    {
        float maxHeightDifference = Mathf.Max(0, heightDifferenceAbs);

        float maxHeight = heightData.Max;
        float minHeight = heightData.Min;
        switch (clippingType)
        {
            case HeightClippingTypes.ClipMin:
                minHeight = Math.Max(maxHeight - maxHeightDifference, minHeight);
                minHeight = (maxHeight - minHeight) * heightDifferenceRel + minHeight;
                break;
            case HeightClippingTypes.ClipMax:
                maxHeight = Math.Min(minHeight + maxHeightDifference, maxHeight);
                maxHeight = maxHeight - (maxHeight - minHeight) * heightDifferenceRel;
                break;
            case HeightClippingTypes.ClipBoth:
                float avgHeight = (minHeight + maxHeight) / 2;
                minHeight = Math.Max(avgHeight - maxHeightDifference / 2, minHeight);
                maxHeight = Math.Min(avgHeight + maxHeightDifference / 2, maxHeight);

                float dif = maxHeight - minHeight;
                minHeight = minHeight + (dif) * heightDifferenceRel / 2;
                maxHeight = maxHeight - (dif) * heightDifferenceRel / 2;

                break;
        }

        heightData = heightData.ClipZ(minHeight, maxHeight);

        return heightData;
    }
}
