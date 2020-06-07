using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.UI
{
    class ElementoUI
    {
        public enum anclajeV
        {
            superior,
            centro,
            inferior
        }

        public enum anclajeH
        {
            izquierda,
            centro,
            derecha
        }

        public anclajeH AnclajeHorizontal { get; set; }
        public anclajeV AnclajeVertical { get; set; }

        float posH;
        float posV;


    }
}