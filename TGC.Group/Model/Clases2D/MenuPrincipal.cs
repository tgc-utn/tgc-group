using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Clases2D
{
    class MenuPrincipal
    {
        private String MediaDir;
        private InputDelJugador input;
        private bool juegoAbiertoPorPrimeraVez;
        private Drawer2D drawer;
        private CustomSprite menuPrincipalSprite;
        private CustomSprite FlechitaSeleccion;
        public MenuPrincipal(String mediaDir, InputDelJugador input)
        {
            this.MediaDir = mediaDir;
            this.input = input;
            juegoAbiertoPorPrimeraVez = true;
            drawer = new Drawer2D();
            menuPrincipalSprite = new CustomSprite();
            menuPrincipalSprite.Bitmap = new CustomBitmap(MediaDir + "InicioMenu.jpg", D3DDevice.Instance.Device);
            menuPrincipalSprite.Position = new TGCVector2(-290, -150);
        }
        public void DibujarMenu()
        {
            if (juegoAbiertoPorPrimeraVez)
            {
                juegoAbiertoPorPrimeraVez = !input.HayInputDePausa(); 
                //Usa el mismo Boton para Pausa y Seleccion menu

                drawer.BeginDrawSprite();
                drawer.DrawSprite(menuPrincipalSprite);
                //drawer.DrawSprite(FlechitaSeleccion);
                drawer.EndDrawSprite();
            }
        }


        public void Dispose()
        {
            menuPrincipalSprite.Dispose();
            //FlechitaSeleccion.Dispose();
        }
    }
}
