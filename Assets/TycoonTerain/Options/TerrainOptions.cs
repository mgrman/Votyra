using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class TerrainOptions : IDisposable
{
    private const int MAX_CELL_COUNT = 60 * 60;

    public readonly Vector2i CellInGroupCount;
    public readonly Bounds GroupBounds;
    public readonly Range2i RangeZ;

    public readonly IImage2i Image;
    public readonly IImageSampler ImageSampler;
    public readonly ITerrainAlgorithm TerrainAlgorithm;
    public readonly ITerrainMesher TerrainMesher;
    public readonly float Time;
    public readonly bool ComputeAsync;
    public readonly bool FlipTriangles;

    public IList<Vector2i> GroupsToUpdate;

    public TerrainOptions(TerrainGeneratorBehaviour terrainGenerator, IList<Vector2i> groupsToUpdate)
    {
        this.CellInGroupCount = terrainGenerator.CellInGroupCount;
        this.FlipTriangles = terrainGenerator.FlipTriangles;

        if (this.CellInGroupCount.AreaSum > MAX_CELL_COUNT)
        {
            throw new InvalidOperationException("Too many cells in group! Max is 60x60");
        }

        if (terrainGenerator.Image is IImage2i)
        {
            this.Image = terrainGenerator.Image as IImage2i;
        }
        else if (terrainGenerator.Image is IImage2)
        {
            this.Image = new RoundImage(terrainGenerator.Image as IImage2);
        }

        this.ImageSampler = terrainGenerator.Sampler as IImageSampler;
        this.TerrainAlgorithm = terrainGenerator.MeshGenerator as ITerrainAlgorithm;
        this.TerrainMesher = terrainGenerator.TerrainMesher as ITerrainMesher;
        this.Time = UnityEngine.Time.time;



        this.RangeZ = Image.RangeZ;
        this.GroupBounds = new Bounds(new Vector3(CellInGroupCount.x / 2.0f, CellInGroupCount.y / 2.0f, RangeZ.Center), new Vector3(CellInGroupCount.x, CellInGroupCount.y, RangeZ.Size));

        this.GroupsToUpdate = groupsToUpdate;
    }

    public TerrainOptions(TerrainOptions template)
    {
        this.CellInGroupCount = template.CellInGroupCount;
        this.FlipTriangles = template.FlipTriangles;

        this.Image = template.Image;
        this.ImageSampler = template.ImageSampler;
        this.TerrainAlgorithm = template.TerrainAlgorithm;
        this.TerrainMesher = template.TerrainMesher;
        this.Time = template.Time;



        this.RangeZ = template.RangeZ;
        this.GroupBounds = template.GroupBounds;

        if (template.GroupsToUpdate != null)
        {
            this.GroupsToUpdate = Pool.Vector2iListPool.GetObject();

            foreach (var group in template.GroupsToUpdate)
            {
                this.GroupsToUpdate.Add(group);
            }
        }
    }


    public bool IsChanged(TerrainOptions old)
    {
        return old == null ||
            this.CellInGroupCount != old.CellInGroupCount ||
            this.FlipTriangles != old.FlipTriangles ||
            this.Image != old.Image ||
            this.Image.IsAnimated ||
            this.TerrainAlgorithm != old.TerrainAlgorithm ||
            this.ImageSampler != old.ImageSampler ||
            this.RangeZ != old.RangeZ ||
            ((this.GroupsToUpdate == null) != (old.GroupsToUpdate == null)) ||
            !this.GroupsToUpdate.SequenceEqual(old.GroupsToUpdate);
    }

    public bool IsBoundsChanged(TerrainOptions old)
    {
        return old == null ||
            this.CellInGroupCount != old.CellInGroupCount;
    }

    public bool IsValid
    {
        get
        {
            return this.TerrainAlgorithm != null
                && this.CellInGroupCount.Positive
                && this.Image != null
                && this.ImageSampler != null
                && this.TerrainMesher != null;
        }
    }

    public TerrainOptions Clone()
    {
        return new TerrainOptions(this);
    }


    public void Dispose()
    {
        if (GroupsToUpdate != null)
        {
            Pool.Vector2iListPool.ReturnObject(GroupsToUpdate);
            GroupsToUpdate = null;
        }
    }
}
