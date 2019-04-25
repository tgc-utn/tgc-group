using System;
using System.Collections.Generic;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Segment
    {
        public static List<Element> GenerateOf(TGCVector3 origin, TGCVector3 maxPoint, int divisions, List<Element> list)
        {
            var mesh = list.Find(id => true).Mesh; //TODO elements for generate

            return GenerateScaleBoxes(origin, maxPoint, divisions)
                .ConvertAll(scaleBox => ScaleMesh(scaleBox, mesh))
                .ConvertAll(scaledMesh => new Element(scaledMesh.Position, scaledMesh));
        }

        private static List<TgcBoundingAxisAlignBox> GenerateScaleBoxes(TGCVector3 origin, TGCVector3 maxPoint, int divisions)
        {
            float x, y, z, xx, yy, zz, zIncrement, xIncrement;
            var scaleBoxes = new List<TgcBoundingAxisAlignBox>();

            y = origin.Y;
            yy = maxPoint.Y;

            zIncrement = (maxPoint.Z - origin.Z) / divisions;
            xIncrement = (maxPoint.X - origin.X) / divisions;

            z = origin.Z;
            for (int i = 1; i <= divisions; i++)
            {
                x = origin.X;
                zz = origin.Z + i * zIncrement;
                for (int j = 1; j <= divisions; j++)
                {
                    xx = origin.X + j * xIncrement;
                    scaleBoxes.Add(new TgcBoundingAxisAlignBox(new TGCVector3(x, y, z), new TGCVector3(xx, yy, zz)));
                    x = xx;
                }
                z = zz;
            }

            return scaleBoxes;
        }

        private static TgcMesh ScaleMesh(TgcBoundingAxisAlignBox scaleBox, TgcMesh mesh)
        {
            var newMesh = mesh.clone(mesh.Name);
            var boundingBox = newMesh.BoundingBox ?? newMesh.createBoundingBox();

            newMesh.Scale = ScaleOfBoxToBox(boundingBox, scaleBox);
            newMesh.Position = scaleBox.PMin;

            newMesh.updateBoundingBox();
            return newMesh;
        }

        private static TGCVector3 ScaleOfBoxToBox(TgcBoundingAxisAlignBox boundingBox, TgcBoundingAxisAlignBox scaleBox)
        {
            var boundingBoxMax = boundingBox.PMax - boundingBox.PMin;
            var scaleBoxMax = scaleBox.PMax - scaleBox.PMin;

            return new TGCVector3(
                scaleBoxMax.X / boundingBoxMax.X,
                scaleBoxMax.Y / boundingBoxMax.Y,
                scaleBoxMax.Z / boundingBoxMax.Z);
        }
    }
}