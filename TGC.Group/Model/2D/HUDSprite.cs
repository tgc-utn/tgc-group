using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;

namespace TGC.Group.Model._2D
{
    class HUDSprite : HUD
    {
        private CustomSprite sprite;

        public HUDSprite(AnclajeHorizontal anclajeHorizontal, AnclajeVertical anclajeVertical, TGCVector2 desplazamiento, TGCVector2 escala, Drawer2D drawer2D, CustomSprite sprite) : base(anclajeHorizontal, anclajeVertical, desplazamiento, escala, drawer2D)
        {
            this.sprite = sprite;
        }

        private void escalar()
        {
            sprite.Scaling = escala * scalingFactor;
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
                    pos.X = D3DDevice.Instance.Width / 2 - sprite.Scaling.X / 2;
                    break;
                case AnclajeHorizontal.RIGHT:
                    pos.X = D3DDevice.Instance.Width - D3DDevice.Instance.Width * desplazamiento.X - sprite.Scaling.X * sprite.Bitmap.Size.Width;
                    break;
            }
            switch (anclajeVertical)
            {
                case AnclajeVertical.TOP:
                    pos.Y = D3DDevice.Instance.Height * desplazamiento.Y;
                    break;
                case AnclajeVertical.CENTER:
                    pos.Y = D3DDevice.Instance.Height / 2 - sprite.Scaling.Y / 2;
                    break;
                case AnclajeVertical.BOTTOM:
                    pos.Y = D3DDevice.Instance.Height - D3DDevice.Instance.Height * desplazamiento.Y - sprite.Scaling.Y * sprite.Bitmap.Size.Height;
                    break;
            }
            sprite.Position = pos;
        }

        public override void Init()
        {
            escalar();
            trasladar();
        }

        public void Update()
        {

        }

        public override void Render()
        {
            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(sprite);
            drawer2D.EndDrawSprite();
        }
    }
}
