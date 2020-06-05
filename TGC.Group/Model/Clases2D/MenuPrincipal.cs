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
        private enum Opciones {JUGAR,SALIR}
        private String MediaDir;
        private InputDelJugador input;
        private bool juegoAbiertoPorPrimeraVez;
        private Drawer2D drawer;
        private CustomSprite menuPrincipalSprite;
        private CustomSprite FlechitaSeleccion;
        private Opciones Seleccion;
        public MenuPrincipal(String mediaDir, InputDelJugador input)
        {
            this.MediaDir = mediaDir;
            this.input = input;
            juegoAbiertoPorPrimeraVez = true;
            drawer = new Drawer2D();
            menuPrincipalSprite = new CustomSprite();
            menuPrincipalSprite.Bitmap = new CustomBitmap(MediaDir + "InicioMenu.png", D3DDevice.Instance.Device);
            FlechitaSeleccion = new CustomSprite();
            FlechitaSeleccion.Bitmap = new CustomBitmap(MediaDir + "Flechita.png", D3DDevice.Instance.Device);
            //Ubicarlo centrado en la pantalla
            menuPrincipalSprite.Position = new TGCVector2(-290, -150);
            FlechitaSeleccion.Position = new TGCVector2(D3DDevice.Instance.Width / 4, FastMath.Max(D3DDevice.Instance.Height / 2, 0) + 140);
            Seleccion = Opciones.JUGAR;
        }
        public void DibujarMenu()
        {
            moverFlechaAPosicion();
            if (juegoAbiertoPorPrimeraVez)
            {
                //juegoAbiertoPorPrimeraVez = !input.HayInputDePausa(); //Usa el mismo Boton para Pausa y Seleccion menu
                if(input.HayInputDePausa())
                    realizarAccion();

                drawer.BeginDrawSprite();
                drawer.DrawSprite(menuPrincipalSprite);
                drawer.DrawSprite(FlechitaSeleccion);
                drawer.EndDrawSprite();
            }
        }

        private void moverFlechaAPosicion()
        {
            var x = FlechitaSeleccion.Position.X;
            if (input.HayInputArriba())
            {
                FlechitaSeleccion.Position = new TGCVector2(x, D3DDevice.Instance.Height / 2 + 140);
                Seleccion = Opciones.JUGAR;
            }
            else if (input.HayInputABajo())
            {
                FlechitaSeleccion.Position = new TGCVector2(x, D3DDevice.Instance.Height / 2 + 260);
                Seleccion = Opciones.SALIR;
            }
            //TODO: Hacer estas posiciones responsive.
        }
        private void realizarAccion()
        {
            if (Seleccion.Equals(Opciones.JUGAR))
                juegoAbiertoPorPrimeraVez = false;
            else if (Seleccion.Equals(Opciones.JUGAR))
                Program.Cerrar();
                //Ya se que el if de arriba esta de mas pero si despues se agregan nuevas funcionalidades
                //Al menu va a tener que ser asi o con un swith.

        }
        public void Dispose()
        {
            menuPrincipalSprite.Dispose();
            FlechitaSeleccion.Dispose();
        }
    }
}
