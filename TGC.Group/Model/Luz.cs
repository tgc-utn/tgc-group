using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    internal class Luz : ObjetoJuego
    {

        public Color Color { get; private set; }

        public Luz(Color color, TGCVector3 translation = default, float scale = 1, TGCVector3 rotation = default, float angle = 0) : base(TGCBox.fromSize(TGCVector3.One * scale, color).ToMesh(""), translation, rotation, angle)
        {
            this.Color = color;
            mesh.setColor(color);
        }

        public override void Render()
        {
            mesh.Transform = TGCMatrix.Translation(new TGCVector3(translation)) * TGCMatrix.Scaling(10,10,10);
            mesh.Render();
        }
    }
}
