using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Clases2D
{
    class Barra
    {
        private String MediaDir;
        private Drawer2D drawer;
        private CustomSprite SpriteBarraVida;
        private CustomSprite SpriteBarraRoll;
        public Barra(String mediaDir)
        {
            this.MediaDir = mediaDir;
            drawer = new Drawer2D();
            SpriteBarraVida = new CustomSprite();
            SpriteBarraRoll = new CustomSprite();

            SpriteBarraVida.Bitmap = new CustomBitmap(MediaDir + "barraVida.png", D3DDevice.Instance.Device);
            SpriteBarraRoll.Bitmap = new CustomBitmap(MediaDir + "barraVida.png", D3DDevice.Instance.Device);
        }
        public void DibujarBarra()
        {
            int altura = SpriteBarraVida.SrcRect.Height;
            float longitudVida = 100 * D3DDevice.Instance.Width / 800;
            int posicionXBaseDeBarras = Convert.ToInt32(D3DDevice.Instance.Width * 0.01f) + 4;
            int posicionYBarraVida = Convert.ToInt32(D3DDevice.Instance.Height / (1.35f)) - 10;
            int posicionYBarraRoll = Convert.ToInt32(D3DDevice.Instance.Height / (1.25f)) - 10;
            TGCVector2 posicionBaseVida = new TGCVector2(posicionXBaseDeBarras, posicionYBarraVida);
            TGCVector2 posicionFinalVida = new TGCVector2(posicionXBaseDeBarras + longitudVida, posicionYBarraVida);
            TGCVector2 posicionBaseRoll = new TGCVector2(posicionXBaseDeBarras, posicionYBarraRoll);

            SpriteBarraVida.Position = posicionBaseVida;
            SpriteBarraVida.Scaling = new TGCVector2(0.2f, 0.2f);

            SpriteBarraRoll.Position = posicionBaseRoll;
            SpriteBarraRoll.Scaling = new TGCVector2(0.2f, 0.2f);

            drawer.BeginDrawSprite();
            drawer.DrawSprite(SpriteBarraVida);
            drawer.DrawSprite(SpriteBarraRoll);
            drawer.EndDrawSprite();
        
        }


        public void Dispose()
        {
            SpriteBarraVida.Dispose();
        }
    }
}
