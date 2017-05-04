using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model
{
    class Jugador
    {
        //HUD
        private HUDJugador claseHUD;

        //Auto
        public Auto claseAuto;

        //Nombre del jugador
        private string NombreJugador;

        //MediaDir
        private string MediaDir;

        //Nro. de Jugador
        private int NroJugador;

        public Jugador(string NombreJugador, string MediaDir, int NroJugador)
        {
            //Guardo variables
            this.NombreJugador = NombreJugador;
            this.MediaDir = MediaDir;
            this.NroJugador = NroJugador;

            //Creo las clases de HUD y el auto
            this.claseHUD = new HUDJugador (MediaDir, this.NombreJugador, this.NroJugador);
            this.claseAuto = new Auto(this.NroJugador);

            return;
        }

        public float GetVidaJugador()
        {
            return this.claseHUD.GetVidaJugador();
        }

        public void Update()
        {
            this.claseHUD.Update();
        }

        public void Render()
        {
            this.claseHUD.Render();
        }

        public void Dispose()
        {
            this.claseHUD.Dispose();
        }
    }
}
