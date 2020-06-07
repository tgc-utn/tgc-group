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
        float valorAgregadoPorBarra = 1.8f;
        public void Render(int cantidadVida, int cantidadNitro)
        {
            
            int coeficienteDeLongitudDeBarras = D3DDevice.Instance.Width / 800;
            int posicionXBaseDeBarras = Convert.ToInt32(D3DDevice.Instance.Width * 0.02f);//original 0.01f

            float longitudVida = cantidadVida * coeficienteDeLongitudDeBarras * valorAgregadoPorBarra;
            int posicionYBarraVida = Convert.ToInt32(D3DDevice.Instance.Height/(1.35f));
            TGCVector2 posicionBaseVida = new TGCVector2(posicionXBaseDeBarras, posicionYBarraVida);
            TGCVector2 posicionFinalVida = new TGCVector2(posicionXBaseDeBarras + longitudVida, posicionYBarraVida);

            float longitudNitro = cantidadNitro * coeficienteDeLongitudDeBarras * valorAgregadoPorBarra;
            int posicionYBarraNitro = Convert.ToInt32(D3DDevice.Instance.Height / (1.25f));
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
                Width = Convert.ToInt32(0.023f * D3DDevice.Instance.Height),
            };
            barra.Draw(TGCVector2.ToVector2Array(positionList), color);
        }

    }
}
