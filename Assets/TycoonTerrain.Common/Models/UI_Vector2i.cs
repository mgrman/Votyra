using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public struct UI_Vector2i 
{
    public  int x;
    public  int y;

    public UI_Vector2i(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static implicit operator Vector2i(UI_Vector2i val)
    {
        return new Vector2i(val.x, val.y);
    }
}

