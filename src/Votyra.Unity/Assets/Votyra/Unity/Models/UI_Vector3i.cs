using System;

namespace Votyra.Unity.Models
{
    [Serializable]
    public struct UI_Vector3i
    {
        public int x;
        public int y;
        public int z;

        public UI_Vector3i(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}