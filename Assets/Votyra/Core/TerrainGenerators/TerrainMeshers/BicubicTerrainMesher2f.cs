using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public static class BicubicTerrainMesher2f
    {
        private const float MaskLimit = 0f;

        public static void GetResultingMesh(ITerrainMesh mesh, Vector2i group, Vector2i cellInGroupCount, IImage2f image, int subdivision)
        {
            var poolableValuesToFill = PoolableMatrix<Vector3f>.CreateDirty(new Vector2i(subdivision + 1, subdivision + 1));
            var valuesToFill = poolableValuesToFill.RawMatrix;
            var poolableMaskToFill = PoolableMatrix<float>.CreateDirty(new Vector2i(subdivision + 1, subdivision + 1));
            var maskToFill = poolableMaskToFill.RawMatrix;

            var groupPosition = cellInGroupCount * group;

            for (var iix = 0; iix < cellInGroupCount.X; iix++)
            {
                for (var iiy = 0; iiy < cellInGroupCount.Y; iiy++)
                {
                    var cellInGroup = new Vector2i(iix, iiy);

                    var cell = cellInGroup + groupPosition;

                    var step = 1.0f / subdivision;

                    var dataX0Y0 = image.SampleCell(cell - Vector2i.One + new Vector2i(0, 0));
                    var dataX0Y2 = image.SampleCell(cell - Vector2i.One + new Vector2i(0, 2));
                    var dataX2Y0 = image.SampleCell(cell - Vector2i.One + new Vector2i(2, 0));
                    var dataX2Y2 = image.SampleCell(cell - Vector2i.One + new Vector2i(2, 2));

                    var valuesInterMatX0Y0 = dataX0Y0.X0Y0;
                    var valuesInterMatX0Y1 = dataX0Y0.X0Y1;
                    var valuesInterMatX0Y2 = dataX0Y2.X0Y0;
                    var valuesInterMatX0Y3 = dataX0Y2.X0Y1;
                    var valuesInterMatX1Y0 = dataX0Y0.X1Y0;
                    var valuesInterMatX1Y1 = dataX0Y0.X1Y1;
                    var valuesInterMatX1Y2 = dataX0Y2.X1Y0;
                    var valuesInterMatX1Y3 = dataX0Y2.X1Y1;
                    var valuesInterMatX2Y0 = dataX2Y0.X0Y0;
                    var valuesInterMatX2Y1 = dataX2Y0.X0Y1;
                    var valuesInterMatX2Y2 = dataX2Y2.X0Y0;
                    var valuesInterMatX2Y3 = dataX2Y2.X0Y1;
                    var valuesInterMatX3Y0 = dataX2Y0.X1Y0;
                    var valuesInterMatX3Y1 = dataX2Y0.X1Y1;
                    var valuesInterMatX3Y2 = dataX2Y2.X1Y0;
                    var valuesInterMatX3Y3 = dataX2Y2.X1Y1;

                    for (var ix = 0; ix < subdivision + 1; ix++)
                    {
                        for (var iy = 0; iy < subdivision + 1; iy++)
                        {
                            var pos = new Vector2f(step * ix, step * iy);

                            var valueCol0 = Intepolate(valuesInterMatX0Y0, valuesInterMatX1Y0, valuesInterMatX2Y0, valuesInterMatX3Y0, pos.X);
                            var valueCol1 = Intepolate(valuesInterMatX0Y1, valuesInterMatX1Y1, valuesInterMatX2Y1, valuesInterMatX3Y1, pos.X);
                            var valueCol2 = Intepolate(valuesInterMatX0Y2, valuesInterMatX1Y2, valuesInterMatX2Y2, valuesInterMatX3Y2, pos.X);
                            var valueCol3 = Intepolate(valuesInterMatX0Y3, valuesInterMatX1Y3, valuesInterMatX2Y3, valuesInterMatX3Y3, pos.X);
                            var value = Intepolate(valueCol0, valueCol1, valueCol2, valueCol3, pos.Y);
                            valuesToFill[ix, iy] = (pos + cell).ToVector3f(value);

                            if (ix > 0 && iy > 0)
                            {
                                var x00Y00 = valuesToFill[ix - 1 + 0, iy - 1 + 0];
                                var x00Y05 = valuesToFill[ix - 1 + 0, iy - 1 + 1];
                                var x05Y00 = valuesToFill[ix - 1 + 1, iy - 1 + 0];
                                var x05Y05 = valuesToFill[ix - 1 + 1, iy - 1 + 1];
                                var x00Y00Mask = maskToFill[ix - 1 + 0, iy - 1 + 0] >= MaskLimit;
                                var x00Y05Mask = maskToFill[ix - 1 + 0, iy - 1 + 1] >= MaskLimit;
                                var x05Y00Mask = maskToFill[ix - 1 + 1, iy - 1 + 0] >= MaskLimit;
                                var x05Y05Mask = maskToFill[ix - 1 + 1, iy - 1 + 1] >= MaskLimit;

                                mesh.AddQuad(x00Y00Mask ? x00Y00 : (Vector3f?) null, x00Y05Mask ? x00Y05 : (Vector3f?) null, x05Y00Mask ? x05Y00 : (Vector3f?) null, x05Y05Mask ? x05Y05 : (Vector3f?) null);
                            }
                        }
                    }
                }
            }

            poolableValuesToFill.Dispose();
            poolableMaskToFill.Dispose();
        }

        // Monotone cubic interpolation
        // https://en.wikipedia.org/wiki/Monotone_cubic_interpolation
        private static float Intepolate(float y0, float y1, float y2, float y3, float x12Rel)
        {
            // Get consecutive differences and slopes
            var dys0 = y1 - y0;
            var dys1 = y2 - y1;
            var dys2 = y3 - y2;

            // Get degree-1 coefficients
            float c1S1;
            if (dys0 * dys1 <= 0)
                c1S1 = 0;
            else
                c1S1 = 6f / (3f / dys0 + 3f / dys1);

            float c1S2;
            if (dys1 * dys2 <= 0)
                c1S2 = 0;
            else
                c1S2 = 6f / (3f / dys1 + 3f / dys2);

            // Get degree-2 and degree-3 coefficients
            var c3S1 = c1S1 + c1S2 - dys1 - dys1;
            var c2S1 = dys1 - c1S1 - c3S1;

            // Interpolate
            var diff = x12Rel;
            var diffSq = diff * diff;
            return y1 + c1S1 * diff + c2S1 * diffSq + c3S1 * diff * diffSq;
        }
    }
}