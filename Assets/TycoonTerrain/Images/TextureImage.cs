using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class TextureImage : MonoBehaviour, IImage2i
{
    [NonSerialized]
    private Vector2i _oldSize = new Vector2i(34, 2);
    public Vector2i Size = new Vector2i(34, 2);

    [NonSerialized]
    private int[,] Image = new int[34, 2]
    {
             //plane
      { 1, 1},{1,1},

      //slopeX+
      { 0, 1},{0,1},
      //slopeX-
      { 1, 0},{1,0},

      //slopeY+
      { 0, 0},{1,1},
      //slopeY-
      { 1, 1},{0,0},

      //slopeX+Y+
      { -1, 0},{0,1},
      //slopeX-Y-
      { 1, 0},{0,-1},
      //slopeX+Y-
      { 0, -1},{1,0},
      //slopeX-Y+
      { 0, 1},{-1,0},

      //partialUpSlopeX+Y+
      { 0, 0},{0,1},
      //partialUpSlopeX-Y-
      { 1, 0},{0,0},
      //partialUpSlopeX+Y-
      { 0, 0},{1,0},
      //partialUpSlopeX-Y+
      { 0, 1},{0,0},

      //partialDownSlopeX+Y+
      { 0, 1},{1,1},
      //partialDownSlopeX-Y-
      { 1, 1},{1,0},
      //partialDownSlopeX+Y-
      { 1, 0},{1,1},
      //partialDownSlopeX-Y+
      { 1, 1},{0,1},
    };

    private Range2i RangeZ = new Range2i(-1, 1);

    private void Update()
    {
        if (_oldSize != Size)
        {
            var old = Image;
            Image = new int[Size.x, Size.y];
            int commonX = Math.Min(_oldSize.x, Size.x);
            int commonY = Math.Min(_oldSize.y, Size.y);

            for (int x = 0; x < commonX; x++)
            {
                for (int y = 0; y < commonY; y++)
                {
                    Image[x, y] = old[x, y];
                }
            }
            _oldSize = Size;
            RangeZ = RecalculateRange(Image);
        }
    }

    private static Range2i RecalculateRange(int[,] mat)
    {
        int countX = mat.GetCountX();
        int countY = mat.GetCountY();

        int min = int.MaxValue;
        int max = int.MinValue;
        for (int x = 0; x < countX; x++)
        {
            for (int y = 0; y < countY; y++)
            {
                int val = mat[x, y];

                min = Math.Min(min, val);
                max = Math.Max(max, val);
            }
        }
        return new Range2i(min, max);
    }

    #region IImage2i

    private bool _anyChangeSinceLast = false;

    Range2i IImage2i.RangeZ
    {
        get
        {
            return RangeZ;
        }
    }

    public bool IsAnimated
    {
        get
        {
            return _anyChangeSinceLast;
        }
    }

    public int Sample(Vector2i point, float time)
    {
        if (point.x < 0 || point.y < 0 || point.x >= Size.x || point.y >= Size.y)
            return 0;

        return Image[point.x, point.y];
    }

    #endregion
}

