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

        public IEnumerable<Element> GenerateElements(int divisions, SpamRate spamRate, List<Element> list)
        {
            var random = new Random();

            return GenerateCubes(this.cube.PMin, this.cube.PMax, divisions)
                .FindAll(scaleBox => spamRate.spam())
                .ConvertAll(scaleBox => ScaleMesh(scaleBox,list[random.Next(list.Count)].Mesh))
                .ConvertAll(scaledMesh => new Element(scaledMesh));
        }

        public static List<Segment> GenerateSegments(TGCVector3 pMin, TGCVector3 pMax, int quantity)
        {
            var res = new List<Segment>();

            var yIncrement = (pMax.Y - pMin.Y) / quantity;

            for (var i = 0; i < quantity; i++)
            {
                var cube = new Cube(
                    new TGCVector3(pMin.X, pMin.Y + i * yIncrement, pMin.Z),
                    new TGCVector3(pMax.X, pMin.Y + (i+1) * yIncrement, pMax.Z));
                
                res.Add(new Segment(cube));
            }

            return res;
        }

        private static List<Cube> GenerateCubes(TGCVector3 origin, TGCVector3 maxPoint, int divisions)
        {
            var res = new List<Cube>();
            
            var zIncrement = (maxPoint.Z - origin.Z) / divisions;
            var xIncrement = (maxPoint.X - origin.X) / divisions;

            for (var i = 0; i < divisions; i++)
            {
                for (var j = 0; j < divisions; j++)
                {
                    res.Add(
                        new Cube(
                            new TGCVector3(origin.X + j * xIncrement, origin.Y, origin.Z + i * zIncrement), 
                            new TGCVector3(origin.X + (j+1) * xIncrement, maxPoint.Y, origin.Z + (i+1) * zIncrement)));                        
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