using System;
using System.Collections.Generic;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model
{
    internal static class Segment
    {
        public static IEnumerable<Element> GenerateOf(TGCVector3 origin, TGCVector3 maxPoint, int divisions, List<Element> list)
        {
            var mesh = list.Find(id => true).Mesh; //TODO elements for generate

            return GenerateScaleBoxes(origin, maxPoint, divisions)
                .ConvertAll(scaleBox => ScaleMesh(scaleBox, mesh))
                .ConvertAll(scaledMesh => new Element(scaledMesh.Position, scaledMesh));
        }

        private static List<Cube> GenerateScaleBoxes(TGCVector3 origin, TGCVector3 maxPoint, int divisions)
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