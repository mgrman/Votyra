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


        public static void AssertContainsQuad(IReadOnlyCollection<Triangle3i> triangles, string cube, Vector3i a, Vector3i b, Vector3i c, Vector3i d)
        {
            bool contains = false;

            contains = contains || triangles.Contains(new Triangle3i(a, b, c), Triangle3i.OrderInvariantComparer) && triangles.Contains(new Triangle3i(a, b, d), Triangle3i.OrderInvariantComparer);//ab
            contains = contains || triangles.Contains(new Triangle3i(a, b, c), Triangle3i.OrderInvariantComparer) && triangles.Contains(new Triangle3i(a, c, d), Triangle3i.OrderInvariantComparer);//ac
            contains = contains || triangles.Contains(new Triangle3i(a, d, c), Triangle3i.OrderInvariantComparer) && triangles.Contains(new Triangle3i(a, d, b), Triangle3i.OrderInvariantComparer);//ad
            contains = contains || triangles.Contains(new Triangle3i(b, c, a), Triangle3i.OrderInvariantComparer) && triangles.Contains(new Triangle3i(b, c, d), Triangle3i.OrderInvariantComparer);//bc
            contains = contains || triangles.Contains(new Triangle3i(b, d, a), Triangle3i.OrderInvariantComparer) && triangles.Contains(new Triangle3i(b, d, c), Triangle3i.OrderInvariantComparer);//bd
            contains = contains || triangles.Contains(new Triangle3i(c, d, a), Triangle3i.OrderInvariantComparer) && triangles.Contains(new Triangle3i(c, d, b), Triangle3i.OrderInvariantComparer);//cd
            if (!contains)
            {
                throw new AssertionException($"On cube:\r\b{cube.Replace(' ', '\u00A0')}\r\nExpected collection containing quad {a},{b},{c},{d}. Actual:\r\n{(triangles.Any() ? string.Join(", ", triangles) : "<empty>")}");
            }
        }


        public static List<Triangle3i> Evaluate(string cubeString)
        {
            var sampler = new SimpleImageSampler3b();
            var imageMock = new Mock<IImage3f>();
            var cube = SampledData3b.ParseCube(cubeString);
            //            Debug.Log(cube);
            imageMock.Setup(o => o.Sample(It.IsAny<Vector3i>()))
                .Returns<Vector3i>((pos) => cube[pos] ? 1f : 0f);

            var triangles = new List<Triangle3i>();
            var meshMock = new Mock<ITerrainMesh>();
            meshMock.Setup(o => o.AddTriangle(It.IsAny<Vector3>(), It.IsAny<Vector3>(), It.IsAny<Vector3>()))
                .Callback<Vector3, Vector3, Vector3>((a, b, c) =>
                {
                    triangles.Add(new Triangle3i(a, b, c));
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

    }
}