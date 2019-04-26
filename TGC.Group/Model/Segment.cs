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

            return GenerateCubes(this.cube.PMin, this.cube.PMax, divisions)
                .FindAll(scaleBox => spawnRate.hasToSpawn())
                .ConvertAll(scaleBox => ScaleMesh(scaleBox,list[random.Next(list.Count)].Mesh))
                .ConvertAll(scaledMesh => new Element(scaledMesh));
        }

        public static List<Segment> GenerateSegments(TGCVector3 pMin, TGCVector3 pMax, int quantity)
        {
            var res = new List<Segment>();

            var yStep = (pMax.Y - pMin.Y) / quantity;

            for (var i = 0; i < quantity; i++)
            {
                var cube = new Cube(
                    new TGCVector3(pMin.X, pMin.Y + i * yStep, pMin.Z),
                    new TGCVector3(pMax.X, pMin.Y + (i+1) * yStep, pMax.Z));
                
                res.Add(new Segment(cube));
            }

            return res;
        }

        private static List<Cube> GenerateCubes(TGCVector3 origin, TGCVector3 maxPoint, int divisions)
        {
            var res = new List<Cube>();

            var xStep = (maxPoint.X - origin.X) / divisions;
            var zStep = (maxPoint.Z - origin.Z) / divisions;

            for (var zDelta = 0; zDelta < divisions; zDelta++)
            {
                for (var xDelta = 0; xDelta < divisions; xDelta++)
                {
                    res.Add(
                        new Cube(
                            new TGCVector3(origin.X + xDelta * xStep, origin.Y, origin.Z + zDelta * zStep), 
                            new TGCVector3(origin.X + (xDelta+1) * xStep, maxPoint.Y, origin.Z + (zDelta+1) * zStep)));                        
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