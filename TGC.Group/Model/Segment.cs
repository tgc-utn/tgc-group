using System;
using System.Collections.Generic;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model
{
    internal class Segment
    {
        private readonly Cube cube;
        private Segment(Cube cube)
        {
            this.cube = cube;
        }

        public IEnumerable<Element> GenerateElements(int divisions, SpawnRate spawnRate, List<Element> list)
        {
            var random = new Random();

            return GenerateXzCubes(this.cube.PMin, this.cube.PMax, divisions)
                .FindAll(scaleBox => spawnRate.HasToSpawn())
                .ConvertAll(scaleBox => ScaleMesh(scaleBox,list[random.Next(list.Count)].Mesh))
                .ConvertAll(scaledMesh => new Element(scaledMesh));
        }

        public static List<Segment> GenerateSegments(TGCVector3 pMin, TGCVector3 pMax, int divisions)
        {
            return GenerateYCubes(pMin,pMax,divisions).ConvertAll(cube => new Segment(cube));
        }

        private static List<Cube> GenerateYCubes(TGCVector3 pMin, TGCVector3 pMax, int divisions)
        {
            var res = new List<Cube>();

            var yStep = (pMax.Y - pMin.Y) / divisions;

            for (var yDelta = 0; yDelta < divisions; yDelta++)
            {
                res.Add(new Cube(
                    new TGCVector3(pMin.X, pMin.Y + yDelta * yStep, pMin.Z),
                    new TGCVector3(pMax.X, pMin.Y + (yDelta+1) * yStep, pMax.Z)));
            }

            return res;
        }

        private static List<Cube> GenerateXzCubes(TGCVector3 pMin, TGCVector3 pMax, int divisions)
        {
            var res = new List<Cube>();

            var xStep = (pMax.X - pMin.X) / divisions;
            var zStep = (pMax.Z - pMin.Z) / divisions;

            for (var zDelta = 0; zDelta < divisions; zDelta++)
            {
                for (var xDelta = 0; xDelta < divisions; xDelta++)
                {
                    res.Add(
                        new Cube(
                            new TGCVector3(pMin.X + xDelta * xStep, pMin.Y, pMin.Z + zDelta * zStep), 
                            new TGCVector3(pMin.X + (xDelta+1) * xStep, pMax.Y, pMin.Z + (zDelta+1) * zStep)));                        
                }
            }

            return res;
        }

        private static TgcMesh ScaleMesh(Cube scaleCube, TgcMesh mesh)
        {
            var newMesh = mesh.clone(mesh.Name);
            var boundingBox = newMesh.BoundingBox ?? newMesh.createBoundingBox();
            
            newMesh.changeD3dMesh(mesh.D3dMesh);
            newMesh.Scale = ScaleOfBoxToBox(boundingBox, scaleCube);
            newMesh.Position = scaleCube.PMin;

            newMesh.updateBoundingBox();
            return newMesh;
        }

        private static TGCVector3 ScaleOfBoxToBox(TgcBoundingAxisAlignBox boundingBox, Cube scaleCube)
        {
            var boundingBoxMax = boundingBox.PMax - boundingBox.PMin;
            var scaleBoxMax = scaleCube.PMax - scaleCube.PMin;

            return new TGCVector3(
                scaleBoxMax.X / boundingBoxMax.X,
                scaleBoxMax.Y / boundingBoxMax.Y,
                scaleBoxMax.Z / boundingBoxMax.Z);
        }
    }
}