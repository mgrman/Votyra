using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

public class TerainMesher : MonoBehaviour, ITerainMesher
{
    private const int CELL_TO_TRIANGLE = 3 * 2;


    //public IDictionary<Vector2i, ITriangleMesh> Mesh(TerainOptions terainOptions, MatrixWithOffset<HeightData> data)
    //{
    //    var meshes = _triangleMeshesCache;

    //    int cellInGroupCount_x = terainOptions.CellInGroupCount.x;
    //    int cellInGroupCount_y = terainOptions.CellInGroupCount.y;
    //    float cellSize_x = terainOptions.CellSize.x;
    //    float cellSize_y = terainOptions.CellSize.y;
    //    float groupSize_x = terainOptions.GroupSize.x;
    //    float groupSize_y = terainOptions.GroupSize.y;
    //    int trianglesInMesh = terainOptions.CellInGroupCount.AreaSum * CELL_TO_TRIANGLE;
    //    var bounds_center = terainOptions.GroupBounds.center;
    //    var bounds_size = terainOptions.GroupBounds.size;

    //    foreach (var group in terainOptions.GroupsToUpdate)
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

    Vector2i group;
    Vector2 cellSize;
    Vector2 groupSize;
    Vector2 groupPosition;
    public int TriangleCount { get; private set; }
    Vector3 bounds_center;
    Vector3 bounds_size;
    int quadIndex;
    bool flipTriangles;
    ITriangleMesh mesh;

    public void Initialize(TerainOptions terainOptions)
    {
        this.cellSize = terainOptions.CellSize;
        this.groupSize = terainOptions.GroupSize;
        this.TriangleCount = terainOptions.CellInGroupCount.AreaSum * CELL_TO_TRIANGLE;
        this.bounds_center = terainOptions.GroupBounds.center;
        this.bounds_size = terainOptions.GroupBounds.size;
        this.flipTriangles = terainOptions.FlipTriangles;
    }

    public void InitializeGroup(Vector2i group, ITriangleMesh mesh)
    {
        this.group = group;
        this.mesh = mesh;
        var bounds = new Bounds(new Vector3
         (
             bounds_center.x + group.x * groupSize.x,
             bounds_center.y + group.y * groupSize.y,
             bounds_center.z
         ), bounds_size);
        mesh.Clear(bounds);

        this.groupPosition = groupSize * group;
        this.quadIndex = 0;
    }


    public void AddCell(HeightData heightData, IMatrix<HeightData> data, Vector2i cellInGroup)
    {
        if (cellInGroup.x >= 0 && cellInGroup.y >= 0)
        {
            Vector2 position = groupPosition + cellSize * cellInGroup;

            var cell_area = new Rect(position.x, position.y, cellSize.x, cellSize.y);

            mesh.AddQuad(quadIndex, cell_area, heightData, !heightData.KeepSides != flipTriangles);
            quadIndex++;

            var minusXres = data[cellInGroup.x - 1, cellInGroup.y];
            float minusXres_x1y1 = minusXres.x1y1;
            float minusXres_x1y0 = minusXres.x1y0;
            mesh.AddQuad(quadIndex,
                new Vector3(position.x, position.y, heightData.x0y0),
                new Vector3(position.x, position.y + cellSize.y, heightData.x0y1),
                new Vector3(position.x, position.y + cellSize.y, minusXres_x1y1),
                new Vector3(position.x, position.y, minusXres_x1y0), false);
            quadIndex++;

            var minusYres = data[cellInGroup.x, cellInGroup.y - 1];
            float minusYres_x0y1 = minusYres.x0y1;
            float minusYres_x1y1 = minusYres.x1y1;
            mesh.AddQuad(quadIndex,
                new Vector3(position.x + cellSize.x, position.y, heightData.x1y0),
                new Vector3(position.x, position.y, heightData.x0y0),
                new Vector3(position.x, position.y, minusYres_x0y1),
                new Vector3(position.x + cellSize.x, position.y, minusYres_x1y1), false);
            quadIndex++;
        }
    }

}
