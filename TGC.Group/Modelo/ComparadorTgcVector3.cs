using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TGC.Core.Mathematica;

namespace TGC.Group.Modelo
{
    class ComparadorYTgcVector3 : IComparer <TGCVector3>
    {
        public ComparadorYTgcVector3()
        {
        }

        public int Compare(TGCVector3 vector1, TGCVector3 vector2)
        {
            if (vector1.Y > vector2.Y) return 0;
            else return 1;
        }

    }
}
