using System;

namespace Votyra.Unity.Models
{
    [Serializable]
    public struct UI_Vector2i
    {
        public int x;
        public int y;

        public UI_Vector2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
