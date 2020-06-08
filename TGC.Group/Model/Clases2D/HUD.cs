using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class HUD
    {
        private String direccionMarco;
        private Drawer2D drawer;
        private CustomSprite marcoVida;
        private CustomSprite marcoGiro;
        private int posicionXBaseDeBarras;
        private int posicionYBarraVida;
        private int posicionYBarraGiro;
        public HUD (string mediaDir)
        {
            direccionMarco = mediaDir + "barra.png";
            drawer = new Drawer2D();
            posicionXBaseDeBarras = Convert.ToInt32(D3DDevice.Instance.Width * 0.02f); //original 0.01f 
            posicionYBarraVida = Convert.ToInt32(D3DDevice.Instance.Height / 1.35f);
            posicionYBarraGiro = Convert.ToInt32(D3DDevice.Instance.Height / 1.25f);

            marcoVida = new CustomSprite
            {
                Bitmap = new CustomBitmap(direccionMarco, D3DDevice.Instance.Device)
            };
            marcoVida.Position = new TGCVector2(posicionXBaseDeBarras, posicionYBarraVida);
            marcoVida.Scaling = new TGCVector2(D3DDevice.Instance.Width* 0.000390625f, D3DDevice.Instance.Height * 0.00092590476f);

            marcoGiro = new CustomSprite
            {
                Bitmap = new CustomBitmap(direccionMarco, D3DDevice.Instance.Device)
            };

            marcoGiro.Position = new TGCVector2(posicionXBaseDeBarras, posicionYBarraGiro);
        }

        private TGCVector2[] PosicionesDeBarra(int cantidadBarra, float posicionYBarra)
        {
            int coeficienteDeLongitudDeBarra = D3DDevice.Instance.Width / 800;
            float longitudBarra = cantidadBarra * coeficienteDeLongitudDeBarra;

            TGCVector2 posicionBase = new TGCVector2(posicionXBaseDeBarras, posicionYBarra);
            TGCVector2 posicionFinal = new TGCVector2(posicionXBaseDeBarras + longitudBarra, posicionYBarra);

            return new TGCVector2[2] { posicionBase, posicionFinal};
        } 

        private TGCVector2[] PosicionesDeBarraVida(int cantidadVida)
        {
            return PosicionesDeBarra(cantidadVida, posicionYBarraVida);
        }

        private TGCVector2[] PosicionesDeBarraGiro(int cantidadGiro)
        {
            return PosicionesDeBarra(cantidadGiro, posicionYBarraGiro);
        }

        public void Render(int cantidadVida, int cantidadGiro)
        {

            DibujarBarra(PosicionesDeBarraVida(cantidadVida), Color.Red);
            DibujarBarra(PosicionesDeBarraGiro(cantidadGiro), Color.Blue);
            drawer.BeginDrawSprite();
            drawer.DrawSprite(marcoVida);
            //drawer.DrawSprite(marcoGiro);
            drawer.EndDrawSprite();
        }

        private void DibujarBarra(TGCVector2[] posiciones, Color color)
        {
            Line barra = new Line(D3DDevice.Instance.Device)
            {
                Antialias = true,
                Width = Convert.ToInt32(0.023f * D3DDevice.Instance.Height),
            };
            barra.Draw(TGCVector2.ToVector2Array(posiciones), color);


        }

    }
}
