using System.Drawing;
using TGC.Core.Mathematica;
using TGC.Core.Text;

namespace TGC.Group.Model._2D
{
    class HUDTexto : HUD
    {
        public string Texto { set { texto.Text = value; } }
        public Color Color { set { texto.Color = value; } }

        private TgcText2D texto;

        public HUDTexto(AnclajeHorizontal anclajeHorizontal, AnclajeVertical anclajeVertical, TGCVector2 desplazamiento, Drawer2D drawer2D, TgcText2D texto) : base(anclajeHorizontal, anclajeVertical, desplazamiento, new TGCVector2(), drawer2D)
        {
            this.texto = texto;
        }

        protected override TGCVector2 getSize()
        {
            return new TGCVector2(texto.Size.Width,texto.Size.Height);
        }

        public override void Init()
        {
            TGCVector2 pos = trasladar();
            texto.Position = new Point((int)pos.X, (int)pos.Y);
        }
        
        public override void Render()
        {
            texto.render();
        }

        public override void Dispose()
        {
            base.Dispose();
            texto.Dispose();
        }
    }
}
