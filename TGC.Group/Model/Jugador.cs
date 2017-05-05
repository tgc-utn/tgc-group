using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Camara;
using TGC.Core.SceneLoader;

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

        //Camara personal
        public CamaraTW claseCamara;

        public Jugador(string NombreJugador, string MediaDir, int NroJugador)
        {
            //Guardo variables
            this.NombreJugador = NombreJugador;
            this.MediaDir = MediaDir;
            this.NroJugador = NroJugador;

            //Creo las clases de HUD y el auto
            this.claseHUD = new HUDJugador (MediaDir, this.NombreJugador, this.NroJugador);
            this.claseAuto = new Auto(MediaDir, this.NroJugador);

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

        public void Update(bool MoverRuedas, List <TgcMesh> listaMesh, bool Avanzar, bool Frenar, bool Izquierda, bool Derecha, bool Saltar, float ElapsedTime)
        {
            this.claseHUD.Update();
            this.claseAuto.Update(MoverRuedas, Avanzar, Frenar, Izquierda, Derecha, Saltar, ElapsedTime);
            this.claseCamara.Update(listaMesh, this.claseAuto.GetPosition(), this.claseAuto.GetRotationAngle());
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
