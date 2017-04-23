namespace TycoonTerrain.Common.Models
{
    public struct ResultHeightData
    {
        public readonly HeightData data;
        public readonly bool flip;

        public ResultHeightData(HeightData data)
        {
            this.data = data;
            this.flip = GetKeepSides(data);
        }

        public ResultHeightData(int x0y0, int x0y1, int x1y0, int x1y1)
        {
            this.data = new HeightData(x0y0, x0y1, x1y0, x1y1);
            this.flip = GetKeepSides(data);
        }

        public ResultHeightData(HeightData data, bool flip)
        {
            this.data = data;
            this.flip = flip;
        }

        public ResultHeightData(int x0y0, int x0y1, int x1y0, int x1y1, bool flip)
        {
            this.data = new HeightData(x0y0, x0y1, x1y0, x1y1);
            this.flip = flip;
        }

        public static bool GetKeepSides(HeightData data)
        {
            return (data.x0y0 < data.x0y1 && data.x0y0 < data.x1y0) || (data.x1y1 < data.x0y1 && data.x1y1 < data.x1y0) || (data.x0y0 > data.x0y1 && data.x0y0 > data.x1y0) || (data.x1y1 > data.x0y1 && data.x1y1 > data.x1y0);
        }

        public override string ToString()
        {
            return string.Format("data({0}), flip({1})", data, flip);
        }
    }
}