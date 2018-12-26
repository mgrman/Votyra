//using System;
//using Votyra.Core.Models;
//
//namespace Votyra.Core.TerrainGenerators.TerrainMeshers.CellComputers
//{
//    public class BicubicDualSampleInterpolator : BicubicInterpolator
//    {
//        public override void PrepareInterpolation(Vector2i cell, Func<Vector2i, SampledData2hf> sampleFunc)
//        {
//            var data_x0y0 = sampleFunc(cell - Vector2i.One + new Vector2i(0, 0));
//            var data_x0y1 = sampleFunc(cell - Vector2i.One + new Vector2i(0, 1));
//            var data_x0y2 = sampleFunc(cell - Vector2i.One + new Vector2i(0, 2));
//            var data_x1y0 = sampleFunc(cell - Vector2i.One + new Vector2i(1, 0));
//            var data_x1y1 = sampleFunc(cell - Vector2i.One + new Vector2i(1, 1));
//            var data_x1y2 = sampleFunc(cell - Vector2i.One + new Vector2i(1, 2));
//            var data_x2y0 = sampleFunc(cell - Vector2i.One + new Vector2i(2, 0));
//            var data_x2y1 = sampleFunc(cell - Vector2i.One + new Vector2i(2, 1));
//            var data_x2y2 = sampleFunc(cell - Vector2i.One + new Vector2i(2, 2));
//
//            InterpolationMatrix[0, 0] = data_x0y0.x0y0;
//            InterpolationMatrix[0, 1] = (data_x0y0.x0y1 + data_x0y1.x0y0) / 2;
//            InterpolationMatrix[0, 2] = (data_x0y1.x0y1 + data_x0y2.x0y0) / 2;
//            InterpolationMatrix[0, 3] = data_x0y2.x0y1;
//
//            InterpolationMatrix[1, 0] = (data_x1y0.x0y0 + data_x0y0.x1y0) / 2;
//            InterpolationMatrix[1, 1] = data_x1y1.x0y0;
//            InterpolationMatrix[1, 2] = data_x1y1.x0y1;
//            InterpolationMatrix[1, 3] = (data_x1y2.x0y1 + data_x0y2.x1y1) / 2;
//
//            InterpolationMatrix[2, 0] = (data_x1y0.x1y0 + data_x2y0.x0y0) / 2;
//            InterpolationMatrix[2, 1] = data_x1y1.x1y0;
//            InterpolationMatrix[2, 2] = data_x1y1.x1y1;
//            InterpolationMatrix[2, 3] = (data_x1y2.x1y1 + data_x2y2.x0y1) / 2;
//
//            InterpolationMatrix[3, 0] = data_x2y0.x1y0;
//            InterpolationMatrix[3, 1] = (data_x2y0.x0y1 + data_x2y1.x0y0) / 2;
//            InterpolationMatrix[3, 2] = (data_x2y1.x0y1 + data_x2y2.x0y0) / 2;
//            InterpolationMatrix[3, 3] = data_x2y2.x1y1;
//        }
//    }
//}