using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.UtilsGroup;

namespace TGC.Group.Model
{
    class HUDTiempo
    {
        //Tiempo de fin de juego
        private int TiempoDeJuego;
        private bool inicioReloj = true;
        private bool finReloj = false;
        private DateTime TiempoFin;
        
        //Fuente para los jugadores
        private TgcDrawText letraJugadores;

        //Path de media
        private string MediaDir;

        public HUDTiempo(string MediaDirectory, int TiempoDeJuego)
        {
            //Guardo variables
            this.MediaDir = MediaDirectory;
            this.TiempoDeJuego = TiempoDeJuego;

            //Creo la letra para el texto
            this.letraJugadores = new TgcDrawText(D3DDevice.Instance.Device, "Rock it", 10, MediaDir);
        }

        private void DibujarTiempo()
        {
            string Tiempo = "";

            //Calculo el tiempo
            Tiempo = (this.TiempoFin - DateTime.Now).Minutes.ToString().PadLeft(2, '0') + ":" + (this.TiempoFin - DateTime.Now).Seconds.ToString().PadLeft(2, '0');

            if (this.finReloj || GameModel.finReloj)
            {
                TgcDrawText letraGanador = new TgcDrawText(D3DDevice.Instance.Device, "Rock it", 36, MediaDir);
                letraGanador.drawText("FIN DEL JUEGO", (Convert.ToInt32(D3DDevice.Instance.Width) / 2) - 200, (Convert.ToInt32(D3DDevice.Instance.Height) / 2) - 150, Color.Red);

                if (GameModel.nroGanador == 0)
                    letraGanador.drawText("GANASTE!!!", (Convert.ToInt32(D3DDevice.Instance.Width) / 2) - 200, (Convert.ToInt32(D3DDevice.Instance.Height) / 2), Color.Green);
                else
                    letraGanador.drawText("PERDISTE...", (Convert.ToInt32(D3DDevice.Instance.Width) / 2) - 200, (Convert.ToInt32(D3DDevice.Instance.Height) / 2), Color.Red);

                this.letraJugadores.drawText("00:00", Convert.ToInt32(D3DDevice.Instance.Width) - HUDJugador.POSICION_X_BARRA_VIDA + 35, HUDJugador.POSICION_Y_BARRA_VIDA - 40, Color.OrangeRed);
                this.letraJugadores.drawText("Presione la letra X para volver al menú inicial...", (Convert.ToInt32(D3DDevice.Instance.Width) / 2) - 200, (Convert.ToInt32(D3DDevice.Instance.Height) / 2) + 150, Color.OrangeRed);
            }
            else
            {
                this.letraJugadores.drawText(Tiempo, Convert.ToInt32(D3DDevice.Instance.Width) - HUDJugador.POSICION_X_BARRA_VIDA + 35, HUDJugador.POSICION_Y_BARRA_VIDA - 40, Color.OrangeRed);
            }
        }

        private void DibujarGanador(string NombreGanador)
        {
            TgcDrawText letraGanador;

            letraGanador = new TgcDrawText(D3DDevice.Instance.Device, "Rock it", 36, MediaDir);
            letraGanador.drawText("FIN DEL JUEGO", (Convert.ToInt32(D3DDevice.Instance.Width) / 2) - 200, (Convert.ToInt32(D3DDevice.Instance.Height) / 2) - 150, Color.Green);
            letraGanador.drawText("GANÓ " + NombreGanador.Trim(), (Convert.ToInt32(D3DDevice.Instance.Width) / 2) - 200, (Convert.ToInt32(D3DDevice.Instance.Height) / 2), Color.Green);
        }

        private void CalcularFinDeJuego()
        {
            //Inicio el tiempo de juego
            if (this.inicioReloj)
            {
                this.TiempoFin = DateTime.Now.AddMinutes(this.TiempoDeJuego);
                this.inicioReloj = false;
            }

            //Calculo si termino
            if (((this.TiempoFin - DateTime.Now).Minutes < 0) || ((this.TiempoFin - DateTime.Now).Seconds < 0))
                SetFinDeJuego();
        }

        public void SetFinDeJuego()
        {
            this.finReloj = true;
        }

        public bool GetFinDeJuego()
        {
            return this.finReloj;
        }

        public void Update()
        {
            this.CalcularFinDeJuego();
        }

        public void Render()
        {
            this.DibujarTiempo();
        }
    }
}
