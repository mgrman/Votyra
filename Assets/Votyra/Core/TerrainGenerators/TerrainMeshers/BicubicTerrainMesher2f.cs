using Votyra.Core.Images;
using Votyra.Core.ImageSamplers;
using Votyra.Core.Models;
using Votyra.Core.TerrainMeshes;

namespace Votyra.Core.TerrainGenerators.TerrainMeshers
{
    public class BicubicTerrainMesher2f : ITerrainMesher2f
    {
        private const float MaskLimit = 0f;

        private readonly Vector2i _cellInGroupCount;
        private readonly Vector2i _subdivision;

        public BicubicTerrainMesher2f(ITerrainConfig terrainConfig, IInterpolationConfig interpolationConfig)
        {
            _cellInGroupCount = terrainConfig.CellInGroupCount.XY();
            _subdivision = interpolationConfig.MeshSubdivision;
        }

        public void GetResultingMesh(ITerrainMesh2f mesh, Vector2i group, IImage2f image, IMask2e mask)
        {
            var range = Area3f.FromMinAndSize((group * _cellInGroupCount).ToVector3f(image.RangeZ.Min), _cellInGroupCount.ToVector3f(image.RangeZ.Size));
            mesh.Reset(range);

            var poolableValuesToFill = PoolableMatrix2<float>.CreateDirty(new Vector2i(_subdivision.X + 1, _subdivision.Y + 1));
            var valuesToFill = poolableValuesToFill.RawMatrix;
            var poolableMaskToFill = PoolableMatrix2<MaskValues>.CreateDirty(new Vector2i(_subdivision.X + 1, _subdivision.Y + 1));
            var maskToFill = poolableMaskToFill.RawMatrix;

            var groupPosition = _cellInGroupCount * group;

            for (var iix = 0; iix < _cellInGroupCount.X; iix++)
            {
                for (var iiy = 0; iiy < _cellInGroupCount.Y; iiy++)
                {
                    var cellInGroup = new Vector2i(iix, iiy);

                    var cell = cellInGroup + groupPosition;
                    
                    var step = 1.0f / _subdivision;

                    var data_x0y0 = image.SampleCell(cell - Vector2i.One + new Vector2i(0, 0));
                    var data_x0y2 = image.SampleCell(cell - Vector2i.One + new Vector2i(0, 2));
                    var data_x2y0 = image.SampleCell(cell - Vector2i.One + new Vector2i(2, 0));
                    var data_x2y2 = image.SampleCell(cell - Vector2i.One + new Vector2i(2, 2));

                    var valuesInterMat_x0y0 = data_x0y0.x0y0;
                    var valuesInterMat_x0y1 = data_x0y0.x0y1;
                    var valuesInterMat_x0y2 = data_x0y2.x0y0;
                    var valuesInterMat_x0y3 = data_x0y2.x0y1;
                    var valuesInterMat_x1y0 = data_x0y0.x1y0;
                    var valuesInterMat_x1y1 = data_x0y0.x1y1;
                    var valuesInterMat_x1y2 = data_x0y2.x1y0;
                    var valuesInterMat_x1y3 = data_x0y2.x1y1;
                    var valuesInterMat_x2y0 = data_x2y0.x0y0;
                    var valuesInterMat_x2y1 = data_x2y0.x0y1;
                    var valuesInterMat_x2y2 = data_x2y2.x0y0;
                    var valuesInterMat_x2y3 = data_x2y2.x0y1;
                    var valuesInterMat_x3y0 = data_x2y0.x1y0;
                    var valuesInterMat_x3y1 = data_x2y0.x1y1;
                    var valuesInterMat_x3y2 = data_x2y2.x1y0;
                    var valuesInterMat_x3y3 = data_x2y2.x1y1;

                    var mask_x0y0 = mask.SampleCell(cell - Vector2i.One + new Vector2i(0, 0));
                    var mask_x0y2 = mask.SampleCell(cell - Vector2i.One + new Vector2i(0, 2));
                    var mask_x2y0 = mask.SampleCell(cell - Vector2i.One + new Vector2i(2, 0));
                    var mask_x2y2 = mask.SampleCell(cell - Vector2i.One + new Vector2i(2, 2));

                    var maskInterMat_x0y0 = mask_x0y0.x0y0.IsHole() ? -1 : 1;
                    var maskInterMat_x0y1 = mask_x0y0.x0y1.IsHole() ? -1 : 1;
                    var maskInterMat_x0y2 = mask_x0y2.x0y0.IsHole() ? -1 : 1;
                    var maskInterMat_x0y3 = mask_x0y2.x0y1.IsHole() ? -1 : 1;
                    var maskInterMat_x1y0 = mask_x0y0.x1y0.IsHole() ? -1 : 1;
                    var maskInterMat_x1y1 = mask_x0y0.x1y1.IsHole() ? -1 : 1;
                    var maskInterMat_x1y2 = mask_x0y2.x1y0.IsHole() ? -1 : 1;
                    var maskInterMat_x1y3 = mask_x0y2.x1y1.IsHole() ? -1 : 1;
                    var maskInterMat_x2y0 = mask_x2y0.x0y0.IsHole() ? -1 : 1;
                    var maskInterMat_x2y1 = mask_x2y0.x0y1.IsHole() ? -1 : 1;
                    var maskInterMat_x2y2 = mask_x2y2.x0y0.IsHole() ? -1 : 1;
                    var maskInterMat_x2y3 = mask_x2y2.x0y1.IsHole() ? -1 : 1;
                    var maskInterMat_x3y0 = mask_x2y0.x1y0.IsHole() ? -1 : 1;
                    var maskInterMat_x3y1 = mask_x2y0.x1y1.IsHole() ? -1 : 1;
                    var maskInterMat_x3y2 = mask_x2y2.x1y0.IsHole() ? -1 : 1;
                    var maskInterMat_x3y3 = mask_x2y2.x1y1.IsHole() ? -1 : 1;

                    for (var ix = 0; ix < _subdivision.X + 1; ix++)
                    {
                        for (var iy = 0; iy < _subdivision.Y + 1; iy++)
                        {
                            var pos = new Vector2f(step.X * ix, step.Y * iy);

                            var maskCol0 = Intepolate(maskInterMat_x0y0, maskInterMat_x1y0, maskInterMat_x2y0, maskInterMat_x3y0, pos.X);
                            var maskCol1 = Intepolate(maskInterMat_x0y1, maskInterMat_x1y1, maskInterMat_x2y1, maskInterMat_x3y1, pos.X);
                            var maskCol2 = Intepolate(maskInterMat_x0y2, maskInterMat_x1y2, maskInterMat_x2y2, maskInterMat_x3y2, pos.X);
                            var maskCol3 = Intepolate(maskInterMat_x0y3, maskInterMat_x1y3, maskInterMat_x2y3, maskInterMat_x3y3, pos.X);
                            var maskValue = Intepolate(maskCol0, maskCol1, maskCol2, maskCol3, pos.Y);
                            maskToFill[ix, iy] = maskValue >= MaskLimit ? MaskValues.Terrain:MaskValues.Hole;

                            var valueCol0 = Intepolate(valuesInterMat_x0y0, valuesInterMat_x1y0, valuesInterMat_x2y0, valuesInterMat_x3y0, pos.X);
                            var valueCol1 = Intepolate(valuesInterMat_x0y1, valuesInterMat_x1y1, valuesInterMat_x2y1, valuesInterMat_x3y1, pos.X);
                            var valueCol2 = Intepolate(valuesInterMat_x0y2, valuesInterMat_x1y2, valuesInterMat_x2y2, valuesInterMat_x3y2, pos.X);
                            var valueCol3 = Intepolate(valuesInterMat_x0y3, valuesInterMat_x1y3, valuesInterMat_x2y3, valuesInterMat_x3y3, pos.X);
                            var value = Intepolate(valueCol0, valueCol1, valueCol2, valueCol3, pos.Y);
                            valuesToFill[ix, iy] = value;

                            if (ix > 0 && iy > 0)
                            {
                                var x00y00 = valuesToFill[ix - 1 + 0, iy - 1 + 0];
                                var x00y05 = valuesToFill[ix - 1 + 0, iy - 1 + 1];
                                var x05y00 = valuesToFill[ix - 1 + 1, iy - 1 + 0];
                                var x05y05 = valuesToFill[ix - 1 + 1, iy - 1 + 1];
                                var x00y00Mask = maskToFill[ix - 1 + 0, iy - 1 + 0];
                                var x00y05Mask = maskToFill[ix - 1 + 0, iy - 1 + 1];
                                var x05y00Mask = maskToFill[ix - 1 + 1, iy - 1 + 0];
                                var x05y05Mask = maskToFill[ix - 1 + 1, iy - 1 + 1];

                                var subcellValue = new SampledData2f(x00y00, x00y05, x05y00, x05y05);
                                var subcellMask = new SampledMask2e(x00y00Mask, x00y05Mask, x05y00Mask, x05y05Mask);

                                mesh.AddCell(cellInGroup,new Vector2i(ix-1,iy-1), subcellValue, subcellMask);
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
            float c1s1;
            if (dys0 * dys1 <= 0)
                c1s1 = 0;
            else
                c1s1 = 6f / (3f / dys0 + 3f / dys1);

            float c1s2;
            if (dys1 * dys2 <= 0)
                c1s2 = 0;
            else
                c1s2 = 6f / (3f / dys1 + 3f / dys2);

            // Get degree-2 and degree-3 coefficients
            var c3s1 = c1s1 + c1s2 - dys1 - dys1;
            var c2s1 = dys1 - c1s1 - c3s1;

            // Interpolate
            var diff = x12Rel;
            var diffSq = diff * diff;
            return y1 + c1s1 * diff + c2s1 * diffSq + c3s1 * diff * diffSq;
        }
    }
}