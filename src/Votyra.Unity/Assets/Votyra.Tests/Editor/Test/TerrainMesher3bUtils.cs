using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Moq;
using NUnit.Framework;
using UnityEngine;
using Votyra.Images;
using Votyra.ImageSamplers;
using Votyra.Models;
using Votyra.TerrainGenerators.TerrainMeshers;
using Votyra.TerrainGenerators.TerrainMeshers.TerrainMeshes;
using Votyra.Unity.Assets.Votyra.Pooling;

namespace Votyra.Tests
{
    public static class TerrainMesher3bUtils
    {
        public static readonly Vector3i x0y0z0 = new Vector3i(0, 0, 0);
        public static readonly Vector3i x0y0z1 = new Vector3i(0, 0, 1);
        public static readonly Vector3i x0y1z0 = new Vector3i(0, 1, 0);
        public static readonly Vector3i x0y1z1 = new Vector3i(0, 1, 1);
        public static readonly Vector3i x1y0z0 = new Vector3i(1, 0, 0);
        public static readonly Vector3i x1y0z1 = new Vector3i(1, 0, 1);
        public static readonly Vector3i x1y1z0 = new Vector3i(1, 1, 0);
        public static readonly Vector3i x1y1z1 = new Vector3i(1, 1, 1);

        const string CubeRegex = @"[^01]*([01])[^01]*([01])[^01]*([01])[^01]*([01])[^01]*([01])[^01]*([01])[^01]*([01])[^01]*([01])[^01]*";

        public static void AssertContainsQuad(IReadOnlyCollection<Triangle> triangles, string cube, Vector3i a, Vector3i b, Vector3i c, Vector3i d)
        {
            bool contains = false;

            contains = contains || triangles.Contains(new Triangle(a, b, c)) && triangles.Contains(new Triangle(a, b, d));//ab
            contains = contains || triangles.Contains(new Triangle(a, b, c)) && triangles.Contains(new Triangle(a, c, d));//ac
            contains = contains || triangles.Contains(new Triangle(a, d, c)) && triangles.Contains(new Triangle(a, d, b));//ad
            contains = contains || triangles.Contains(new Triangle(b, c, a)) && triangles.Contains(new Triangle(b, c, d));//bc
            contains = contains || triangles.Contains(new Triangle(b, d, a)) && triangles.Contains(new Triangle(b, d, c));//bd
            contains = contains || triangles.Contains(new Triangle(c, d, a)) && triangles.Contains(new Triangle(c, d, b));//cd
            if (!contains)
            {
                throw new AssertionException($"On cube:\r\b{cube.Replace(' ', '\u00A0')}\r\nExpected collection containing quad {a},{b},{c},{d}. Actual:\r\n{(triangles.Any() ? string.Join(", ", triangles) : "<empty>")}");
            }
        }


        public static List<Triangle> Evaluate(string cubeString)
        {
            var sampler = new SimpleImageSampler3b();
            var imageMock = new Mock<IImage3b>();
            var cube = ParseCube(cubeString);
            //            Debug.Log(cube);
            imageMock.Setup(o => o.Sample(It.IsAny<Vector3i>()))
                .Returns<Vector3i>((pos) => cube[pos]);

            var triangles = new List<Triangle>();
            var meshMock = new Mock<ITerrainMesh>();
            meshMock.Setup(o => o.AddTriangle(It.IsAny<Vector3>(), It.IsAny<Vector3>(), It.IsAny<Vector3>()))
                .Callback<Vector3, Vector3, Vector3>((a, b, c) =>
                {
                    triangles.Add(new Triangle(a, b, c));
                });
            var mesh = meshMock.Object;

            var pooledMeshMock = new Mock<IPooledTerrainMesh>();
            pooledMeshMock.Setup(o => o.Mesh)
            .Returns(() => mesh);

            var mesher = new TerrainMesher3b();
            mesher.Initialize(sampler, imageMock.Object, new Vector3i(1, 1, 1));
            mesher.InitializeGroup(new Vector3i(0, 0, 0), pooledMeshMock.Object);

            mesher.AddCell(new Vector3i(0, 0, 0));
            return triangles;
        }


        public static SampledData3b ParseCube(string cube)
        {
            var match = Regex.Match(cube, CubeRegex);
            bool val_x0y0z0 = int.Parse(match.Groups[7].Value) != 0;
            bool val_x0y0z1 = int.Parse(match.Groups[3].Value) != 0;
            bool val_x0y1z0 = int.Parse(match.Groups[5].Value) != 0;
            bool val_x0y1z1 = int.Parse(match.Groups[1].Value) != 0;
            bool val_x1y0z0 = int.Parse(match.Groups[8].Value) != 0;
            bool val_x1y0z1 = int.Parse(match.Groups[4].Value) != 0;
            bool val_x1y1z0 = int.Parse(match.Groups[6].Value) != 0;
            bool val_x1y1z1 = int.Parse(match.Groups[2].Value) != 0;
            return new SampledData3b(val_x0y0z0, val_x0y0z1, val_x0y1z0, val_x0y1z1, val_x1y0z0, val_x1y0z1, val_x1y1z0, val_x1y1z1);
        }

        public static string ToCubeString(this SampledData3b data)
        {
            return $@"
              {(data.Data_x0y1z1 ? 1 : 0)}-----{(data.Data_x1y1z1 ? 1 : 0)}
             /|    /|
            {(data.Data_x0y0z1 ? 1 : 0)}-+---{(data.Data_x1y0z1 ? 1 : 0)} |
            | {(data.Data_x0y1z0 ? 1 : 0)}---+-{(data.Data_x1y1z0 ? 1 : 0)}
            |/    |/
            {(data.Data_x0y0z0 ? 1 : 0)}-----{(data.Data_x1y0z0 ? 1 : 0)}
            ";
        }

        public struct Triangle
        {
            public readonly Vector3i a;
            public readonly Vector3i b;
            public readonly Vector3i c;

            public Triangle(Vector3i a, Vector3i b, Vector3i c)
            {
                this.a = a;
                this.b = b;
                this.c = c;
            }

            public Triangle(Vector3 a, Vector3 b, Vector3 c)
            {
                this.a = new Vector3i(a);
                this.b = new Vector3i(b);
                this.c = new Vector3i(c);
            }

            public IEnumerable<Vector3i> Points
            {
                get
                {

                    yield return a;
                    yield return b;
                    yield return c;
                }
            }

            public override bool Equals(object obj)
            {
                if (obj == null || !(obj is Triangle))
                {
                    return false;
                }

                var that = (Triangle)obj;
                foreach (var point in this.Points)
                {
                    if (!that.Points.Contains(point))
                    {
                        return false;
                    }
                }
                return true;
            }

            public override int GetHashCode()
            {
                return (a + b + c).GetHashCode();
            }
            public override string ToString()
            {
                return $"{a},{b},{c}";
            }
        }
    }
}