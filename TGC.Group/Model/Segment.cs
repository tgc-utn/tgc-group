using System;
using System.Collections.Generic;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class Segment
    {
        public static List<Element> GenerateOf(TGCVector3 origin, TGCVector3 maxPoint, int divisions, List<Element> elements)
        {
            float x, minY, maxY, z, i, j;

            x = origin.X;
            minY = origin.Y;
            maxY = maxPoint.Y;
            z = origin.Z;

            List<TgcBoundingAxisAlignBox> res = new List<TgcBoundingAxisAlignBox>();

            for (i=1; i<=divisions; i++)
            {
                for(j = 1; j<=divisions; j++)
                {
                    res.Add(new TgcBoundingAxisAlignBox(new TGCVector3(x, minY, z), new TGCVector3(x, maxY, z)));
                    x = x + (j * maxPoint.X / divisions);
                }
                z = z + (i * maxPoint.Z / divisions);
            }

            //TODO hacerlo bien
            Element lala = elements.Find(element=>true);

            TgcBoundingAxisAlignBox lili, lele = lala.getCollisionVolume();
            lili = res.Find(element => true);

            List<Element> realRes = new List<Element>();
            foreach(var el in res)
            {
                float xx, yy, zz;
                TGCVector3 max;
                xx = lele.PMax.X - lele.PMin.X;
                yy = lele.PMax.Y - lele.PMin.Y;
                zz = lele.PMax.Z - lele.PMin.Z;

                max = new TGCVector3(el.PMin.X + xx, el.PMin.Y + yy, el.PMin.Z + zz);

                Element toAdd = new Element(el.PMin, lala.model, new TGCVector3(el.PMax.X / max.X, el.PMax.Y / max.Y, el.PMax.Z / max.Z));

                realRes.Add(toAdd);
            }

            return realRes;
        }
    }
}