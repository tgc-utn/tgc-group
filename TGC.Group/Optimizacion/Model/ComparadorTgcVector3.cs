using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class ComparadorYTgcVector3 : IComparer <TGCVector3>
    {
        public ComparadorYTgcVector3()
        {
        }

        public int Compare(TGCVector3 x, TGCVector3 y)
        {
            if (x.Y > y.Y) return 0;
            else return 1;
        }

    }
}
