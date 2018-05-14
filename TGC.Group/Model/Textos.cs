using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Text;
using TGC.Core.Mathematica;
using System.Drawing;

namespace TGC.Group.Model
{
    class Textos
    {
        public static TgcText2D newText(string formato, int x, int y)
        {
            TgcText2D texto = new TgcText2D();
            texto.Text = formato;
            texto.Position = new Point(x, y);
            texto.Size = new Size(0, 0);
            texto.Color = Color.Gold;
            return texto;
        }
    }
}
