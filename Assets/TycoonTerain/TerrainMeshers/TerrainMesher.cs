using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

public class TerrainMesher : MonoBehaviour, ITerrainMesher
{
    private const int CELL_TO_TRIANGLE = 3 * 2;


    //public IDictionary<Vector2i, ITriangleMesh> Mesh(TerrainOptions terrainOptions, MatrixWithOffset<HeightData> data)
    //{
    //    var meshes = _triangleMeshesCache;

    //    int cellInGroupCount_x = terrainOptions.CellInGroupCount.x;
    //    int cellInGroupCount_y = terrainOptions.CellInGroupCount.y;
    //    float cellSize_x = terrainOptions.CellSize.x;
    //    float cellSize_y = terrainOptions.CellSize.y;
    //    float groupSize_x = terrainOptions.GroupSize.x;
    //    float groupSize_y = terrainOptions.GroupSize.y;
    //    int trianglesInMesh = terrainOptions.CellInGroupCount.AreaSum * CELL_TO_TRIANGLE;
    //    var bounds_center = terrainOptions.GroupBounds.center;
    //    var bounds_size = terrainOptions.GroupBounds.size;

    //    foreach (var group in terrainOptions.GroupsToUpdate)
    //    {
    //        ITriangleMesh mesh;
    //        if (!meshes.TryGetValue(group, out mesh))
    //        {
    //            mesh = new FixedTriangleMesh(trianglesInMesh);
    //            meshes[group] = mesh;
    //        }
    //        var bounds = new Bounds(new Vector3
    //         (
    //             bounds_center.x + group.x * groupSize_x,
    //             bounds_center.y + group.y * groupSize_y,
    //             bounds_center.z
    //         ), bounds_size);
    //        mesh.Clear(bounds);


    //        int quadIndex = 0;

    //        //int cell_y_inLastGroup = cell_y;

    //        for (int cellInGroup_x = -1; cellInGroup_x < cellInGroupCount_x; cellInGroup_x++)
    //        {
    //            //cell_x++;

    //            //cell_y = cell_y_inLastGroup;

    //            int cell_x = group.x * cellInGroupCount_x + cellInGroup_x;

    //            float x = cell_x * cellSize_x;
    //            for (int cellInGroup_y = -1; cellInGroup_y < cellInGroupCount_y; cellInGroup_y++)
    //            {
    //                int cell_y = group.y * cellInGroupCount_y + cellInGroup_y;
    //                float y = cell_y * cellSize_y;

    //                //Debug.Log(string.Format("Processing cell {0}x{1}", cell_x, cell_y));



    //                //cell_y++;
    //            }
    //        }
    //        //}
    //    }

    //    return meshes;
    //}
    
    Vector2i cellInGroupCount;
    Vector2i groupPosition;
    public int TriangleCount { get; private set; }
    Vector3 bounds_center;
    Vector3 bounds_size;
    int quadIndex;
    bool flipTriangles;
    ITriangleMesh mesh;

    public void Initialize(TerrainOptions terrainOptions)
    {
        this.cellInGroupCount = terrainOptions.CellInGroupCount;
        this.TriangleCount = terrainOptions.CellInGroupCount.AreaSum * CELL_TO_TRIANGLE;
        this.bounds_center = terrainOptions.GroupBounds.center;
        this.bounds_size = terrainOptions.GroupBounds.size;
        this.flipTriangles = terrainOptions.FlipTriangles;
    }

    public void InitializeGroup(Vector2i group, ITriangleMesh mesh)
    {
        this.mesh = mesh;
        var bounds = new Bounds(new Vector3
         (
             bounds_center.x + group.x * cellInGroupCount.x,
             bounds_center.y + group.y * cellInGroupCount.y,
             bounds_center.z
         ), bounds_size);
        mesh.Clear(bounds);

        this.groupPosition = cellInGroupCount * group;
        this.quadIndex = 0;
    }


    public void AddCell(HeightData heightData, IMatrix<HeightData> data, Vector2i cellInGroup)
    {
        if (cellInGroup.x >= 0 && cellInGroup.y >= 0)
        {
            Vector2i position = groupPosition +  cellInGroup;

            var cell_area = new Rect(position.x, position.y, 1, 1);

            mesh.AddQuad(quadIndex, cell_area, heightData, !heightData.KeepSides != flipTriangles);
            quadIndex++;

            var minusXres = data[cellInGroup.x - 1, cellInGroup.y];
            float minusXres_x1y1 = minusXres.x1y1;
            float minusXres_x1y0 = minusXres.x1y0;
            mesh.AddQuad(quadIndex,
                new Vector3(position.x, position.y, heightData.x0y0),
                new Vector3(position.x, position.y + 1, heightData.x0y1),
                new Vector3(position.x, position.y + 1, minusXres_x1y1),
                new Vector3(position.x, position.y, minusXres_x1y0), false);
            quadIndex++;

            var minusYres = data[cellInGroup.x, cellInGroup.y - 1];
            float minusYres_x0y1 = minusYres.x0y1;
            float minusYres_x1y1 = minusYres.x1y1;
            mesh.AddQuad(quadIndex,
                new Vector3(position.x + 1, position.y, heightData.x1y0),
                new Vector3(position.x, position.y, heightData.x0y0),
                new Vector3(position.x, position.y, minusYres_x0y1),
                new Vector3(position.x + 1, position.y, minusYres_x1y1), false);
            quadIndex++;
        }
    }

}
