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
            float x, y, yy, z, xx, zz;
            y = origin.Y;
            yy = maxPoint.Y;

            var boundingBoxes = new List<TgcBoundingAxisAlignBox>();
            TgcBoundingAxisAlignBox newBox;

            z = origin.Z;
            for (int i=1; i<= divisions; i++)
            {
                x = origin.X;
                zz = i * maxPoint.Z / divisions;
                for (int j = 1; j <= divisions; j++)
                {
                    xx = j * maxPoint.X / divisions;
                    newBox = new TgcBoundingAxisAlignBox(new TGCVector3(x, y, z), new TGCVector3(xx , yy, zz));
                    boundingBoxes.Add(newBox);
                    x = xx;
                }
                z = zz;
            }

            var box = list.Find(id => true);

            var res = new List<Element>();

            foreach(var aBox in boundingBoxes.GetRange(0,1))
            {
                var model = box.Model.clone("box");
                var boundingModel = model.createBoundingBox();

                TGCVector3 absoluteMax, newMax, scale;

                absoluteMax = boundingModel.PMax - boundingModel.PMin;
                newMax = aBox.PMin + absoluteMax;
                scale = new TGCVector3(
                    (aBox.PMax.X - aBox.PMin.X) / absoluteMax.X,
                    (aBox.PMax.Y - aBox.PMin.Y) / absoluteMax.Y,
                    (aBox.PMax.Z - aBox.PMin.Z) / absoluteMax.Z);

                model.Scale = scale;
                model.updateBoundingBox();
                res.Add(new Element(
                    new TGCVector3(aBox.PMin.X + aBox.PMin.X/2,
                    aBox.PMin.Y + aBox.PMin.Y / 2,
                    aBox.PMin.Z + aBox.PMin.Z / 2), model));
            }

            return res;
        }
    }
}