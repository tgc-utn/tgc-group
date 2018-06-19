using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using TGC.Core.Text;
using TGC.Core.Mathematica;

namespace TGC.Group.Modelo
{
    public class Informador
    {
        private TgcText2D drawer;
        private float ScreenRes_X = 0f;
        private float ScreenRes_Y = 0f;
        private float tiempoInforme =3f;

        private TGCVector2 posicionInforme = new TGCVector2(500, 500);

        //Eventos
        private bool checkpoint = false;
        private bool hogueraCercana = false;

        public Informador(TgcText2D drawer,float ScreenRes_X, float ScreenRes_Y)
        {
            this.ScreenRes_X = ScreenRes_X;
            this.ScreenRes_Y = ScreenRes_Y;

            this.drawer = drawer;
        }
        public void informar(EstadoJuego estado,Personaje personaje, float ElapsedTime)
        {
            var mensaje = "";

            renderizarControles();
            renderizarDebug();
            drawer.drawText((estado.godMode ? "GOD MODE: ON" : ""), (int)(ScreenRes_X - 140f), 50, Color.Red);

            if(checkpoint)
            {
                checkpoint = false;
                mensaje = "Nuevo Checkpoint";
            }
            if(hogueraCercana)
            {
                mensaje = "Hoguera Cercana, apriete E para encederla";
                hogueraCercana = false;
            }

            drawer.drawText(mensaje, (int)posicionInforme.X, (int)posicionInforme.Y, Color.Orange);
        }

        public void renderizarControles()
        {
            drawer.drawText("Mover Hacia Adelante: W" + "\n"
                               + "Mover Hacia Atras: S" + "\n"
                               + "Mover Hacia Derecha: D" + "\n"
                               + "Mover Hacia Izquierda: A" + "\n"
                               + "Saltar: Barra Espaciadora" + "\n"
                               + "Patear: Q" + "\n"
                               + "Empujar: R" + "\n"
                               + "Prender Hoguera: E" + "\n"
                               + "Pausar/Reanudar Sonido: Z" + "\n"
                               + "Menu: M" + "\n"
                                , 500, 0, Color.Green);
        }

        public void renderizarDebug()
        {
            drawer.drawText("Hoguera Cercana: " + hogueraCercana + "\n"
                            + "Nuevo Checkpoint: " + checkpoint + "\n"
                                , 500, 500,Color.Green);
        }

        public void nuevoCheckpoint()
        {
            checkpoint = true;
        }

        public void hogueraCerca()
        {
            hogueraCercana = true;
        }
    }
}
