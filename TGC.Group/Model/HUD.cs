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
        public void Render(int cantidadVida, int cantidadNitro)
        {
            int posicionXBaseDeBarras = 25;
            float longitudVida = cantidadVida * 2.5f;
            int posicionYBarraVida = 800;
            TGCVector2 posicionBaseVida = new TGCVector2(posicionXBaseDeBarras, posicionYBarraVida);
            TGCVector2 posicionFinalVida = new TGCVector2(posicionXBaseDeBarras + longitudVida, posicionYBarraVida);

            float longitudNitro = cantidadNitro * 2.5f;
            int posicionYBarraNitro = posicionYBarraVida + 50;
            TGCVector2 posicionBaseNitro = new TGCVector2(posicionXBaseDeBarras, posicionYBarraNitro);
            TGCVector2 posicionFinalNitro = new TGCVector2(posicionXBaseDeBarras + longitudNitro, posicionYBarraNitro);

            DibujarBarra(posicionBaseVida, posicionFinalVida, Color.Red);
            DibujarBarra(posicionBaseNitro, posicionFinalNitro, Color.Blue);
        }
        private void DibujarBarra(TGCVector2 position1, TGCVector2 position2, Color color)
        {
            var positionList = new TGCVector2[2] { position1, position2 };
            Line barra = new Line(D3DDevice.Instance.Device)
            {
                Antialias = true,
                Width = 25
            };
            barra.Draw(TGCVector2.ToVector2Array(positionList), color);
        }

    }
}
