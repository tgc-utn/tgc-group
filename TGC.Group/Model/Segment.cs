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

            for (var i = 0; i < quantity; i++)
            {
                var cube = new Cube(
                    new TGCVector3(pMin.X, pMin.Y + pMax.Y * i / quantity, pMin.Z),
                    new TGCVector3(pMax.X, pMin.Y + pMax.Y * (i + 1) / quantity, pMax.Z));
                
                res.Add(new Segment(cube));
            }

            return res;
        }

        private static List<Cube> GenerateCubes(TGCVector3 origin, TGCVector3 maxPoint, int divisions)
        {
            var scaleBoxes = new List<Cube>();

            var y = origin.Y;
            var yy = maxPoint.Y;

            var zIncrement = (maxPoint.Z - origin.Z) / divisions;
            var xIncrement = (maxPoint.X - origin.X) / divisions;

            var z = origin.Z;
            for (var i = 1; i <= divisions; i++)
            {
                var x = origin.X;
                var zz = origin.Z + i * zIncrement;
                for (var j = 1; j <= divisions; j++)
                {
                    var xx = origin.X + j * xIncrement;
                    scaleBoxes.Add(new Cube(new TGCVector3(x, y, z), new TGCVector3(xx, yy, zz)));                        
                    x = xx;
                }
                z = zz;
            }

            return scaleBoxes;
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