using System;
using System.Collections.Generic;
using System.Linq;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Utils;
using BulletSharp;

namespace TGC.Group.Model
{
    internal class Segment
    {
        private readonly Cube cube;
        private Segment(Cube cube)
        {
            this.cube = cube;
        }

        public IEnumerable<Element> GenerateElements(int divisions, SpawnRate spawnRate, List<TgcMesh> list)
        {
            var random = new Random();

            return ElementsToSpawn(divisions, spawnRate)
                .ConvertAll(scaleBox => ScaleMesh(scaleBox, list[random.Next(list.Count)]))
                .ConvertAll(scaledMesh => GenerateElement(scaledMesh));
        }

        private static Element GenerateElement(TgcMesh scaledMesh)
        {
            return new Element(scaledMesh, RigidBodyFactory.CreateCapsule(scaledMesh));
        }

        private List<Cube> ElementsToSpawn(int divisions, SpawnRate spawnRate)
        {
            return GenerateXzCubes(this.cube.PMin, this.cube.PMax, divisions)
                            .FindAll(scaleBox => spawnRate.HasToSpawn());
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
            var newMesh = mesh.createMeshInstance(mesh.Name);
            
            newMesh.Scale = ScaleOfBoxToBox(newMesh.BoundingBox, scaleCube);
            newMesh.Position = scaleCube.PMin;
            newMesh.UpdateMeshTransform();
            newMesh.updateBoundingBox();




            return newMesh;
        }

        private static TGCVector3 ScaleOfBoxToBox(TgcBoundingAxisAlignBox boundingBox, Cube scaleCube)
        {
            var boundingBoxMax = boundingBox.PMax - boundingBox.PMin;
            var scaleBoxMax = scaleCube.PMax - scaleCube.PMin;

            var minScale = new[]
            {
                scaleBoxMax.X / boundingBoxMax.X,
                scaleBoxMax.Y / boundingBoxMax.Y,
                scaleBoxMax.Z / boundingBoxMax.Z
            }.Min();
            
            return new TGCVector3(minScale,minScale,minScale);
        }
    }
}