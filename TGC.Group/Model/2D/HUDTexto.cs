using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Text;

namespace TGC.Group.Model._2D
{
    class HUDTexto : HUD
    {
        private TgcText2D texto;

        public HUDTexto(AnclajeHorizontal anclajeHorizontal, AnclajeVertical anclajeVertical, TGCVector2 desplazamiento, TGCVector2 escala, Drawer2D drawer2D, TgcText2D texto) : base(anclajeHorizontal, anclajeVertical, desplazamiento, escala, drawer2D)
        {
            this.texto = texto;
        }

        private void trasladar()
        {
            TGCVector2 pos = new TGCVector2();
            switch (anclajeHorizontal)
            {
                case AnclajeHorizontal.LEFT:
                    pos.X = D3DDevice.Instance.Width * desplazamiento.X;
                    break;
                case AnclajeHorizontal.CENTER:
                    pos.X = D3DDevice.Instance.Width / 2 - texto.Size.Width / 2;
                    break;
                case AnclajeHorizontal.RIGHT:
                    pos.X = D3DDevice.Instance.Width - D3DDevice.Instance.Width * desplazamiento.X - texto.Size.Width;
                    break;
            }
            switch (anclajeVertical)
            {
                case AnclajeVertical.TOP:
                    pos.Y = D3DDevice.Instance.Height * desplazamiento.Y;
                    break;
                case AnclajeVertical.CENTER:
                    pos.Y = D3DDevice.Instance.Height / 2 - texto.Size.Height / 2;
                    break;
                case AnclajeVertical.BOTTOM:
                    pos.Y = D3DDevice.Instance.Height - D3DDevice.Instance.Height * desplazamiento.Y - texto.Size.Height;
                    break;
            }
            texto.Position = new Point((int)pos.X, (int)pos.Y);
        }

        public override void Init()
        {
            trasladar();
        }

        public void Update()
        {

        }

        public override void Render()
        {
            texto.render();
        }
    }
}
