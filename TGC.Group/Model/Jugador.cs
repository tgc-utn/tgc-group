using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Camara;
using TGC.Core.SceneLoader;
using TGC.Core.Sound;

namespace TGC.Group.Model
{
    class Jugador
    {
        //HUD
        public HUDJugador claseHUD;

        //Auto
        public Auto claseAuto;

        //Nombre del jugador
        private string NombreJugador;

        //MediaDir
        private string MediaDir;

        //Nro. de Jugador
        private int NroJugador;

        //Camara personal
        public CamaraTW claseCamara;

        public Jugador(string NombreJugador, string MediaDir, int NroJugador, TgcDirectSound directSound)
        {
            //Guardo variables
            this.NombreJugador = NombreJugador;
            this.MediaDir = MediaDir;
            this.NroJugador = NroJugador;

            //Creo las clases de HUD y el auto
            this.claseHUD = new HUDJugador (MediaDir, this.NombreJugador, this.NroJugador);
            this.claseAuto = new Auto(MediaDir, this.NroJugador, directSound);

            return;
        }

        public void CreateCamera()
        {
            this.claseCamara = new CamaraTW(this.claseAuto.GetPosition());
        }

        public float GetVidaJugador()
        {
            return this.claseHUD.GetVidaJugador();
        }

        public int GetNroJugador()
        {
            return this.NroJugador;
        }

        public void ActualizarNombreJugador(string NombreJugador)
        {
            this.NombreJugador = NombreJugador;
            this.claseHUD.ActualizarNombreJugador(NombreJugador);
        }

        public void Seguir(List<Jugador> listaJugadores, float ElapsedTime)
        {
            //var autoRival = otroJugador.claseAuto;
            var otroJugador = listaJugadores[0];
            var otroAuto = otroJugador.claseAuto;

            if (this.GetVidaJugador() > 0)
            {
                this.claseAuto.Seguir(otroAuto, ElapsedTime);
            }
            else
            {
                this.claseAuto.MOVEMENT_SPEED = 0;
            }
        }

        public void Update(bool MoverRuedas, bool Avanzar, bool Frenar, bool Izquierda, bool Derecha, bool Saltar, float ElapsedTime)
        {
            this.claseHUD.SetVidaJugador(this.claseAuto.ModificadorVida);
            this.claseAuto.ModificadorVida = 0;
            this.claseHUD.Update();
            this.claseAuto.Update(MoverRuedas, Avanzar, Frenar, Izquierda, Derecha, Saltar, ElapsedTime, this.claseHUD.GetVidaJugador());
            this.claseCamara.Update(this.claseAuto.GetPosition(), this.claseAuto.GetRotationAngle());
        }

        public void Render()
        {
            this.claseHUD.Render();
            this.claseAuto.Render();
            this.claseCamara.Render();
        }

        public void Dispose()
        {
            this.claseHUD.Dispose();
            this.claseAuto.Dispose();
            this.claseCamara.Dispose();
        }
    }
}
