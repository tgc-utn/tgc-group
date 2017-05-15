using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Textures;
using TGC.UtilsGroup;

namespace TGC.Group.Model
{
    class HUDJugador
    {
        //Sprites para las barras de vida
        private TgcDrawer2D drawerBarras;
        private TgcSprite spriteBarraJugador;
        private TgcSprite spriteBarraJugadorLlena;

        //Vida Inicial de cada jugador
        private float cantVidaJugador = 100;

        //Path de media
        private string MediaDir;

        //Posición Y de las barras de vida
        public const int POSICION_Y_BARRA_VIDA = 40;

        //Posición X de las barras de vida
        public const int POSICION_X_BARRA_VIDA = 110;

        //Fuente para los jugadores
        private TgcDrawText letraJugadores;

        //Nombre del jugador
        private string NombreJugador;

        //Multiplicador de posición
        private int MultiplicadorPos;

        public HUDJugador(string MediaDirectory, string NombreJugador, int MultiplicadorPos)
        {
            //Guardo variables
            this.MediaDir = MediaDirectory;
            this.NombreJugador = NombreJugador;
            this.MultiplicadorPos = MultiplicadorPos;

            //Creo el dibujador de las barras
            this.drawerBarras = new TgcDrawer2D(D3DDevice.Instance.Device);

            //Creo la letra para el texto
            this.letraJugadores = new TgcDrawText(D3DDevice.Instance.Device, "Rock it", 10, MediaDir);

            /*
            if (MultiplicadorPos == 0)
                this.cantVidaJugador = 100;

            if (MultiplicadorPos == 1)
                this.cantVidaJugador = 20;

            if (MultiplicadorPos == 2)
                this.cantVidaJugador = 20;

            if (MultiplicadorPos == 3)
                this.cantVidaJugador = 50;

            if (MultiplicadorPos == 4)
                this.cantVidaJugador = 40;
            */

            //Creo las barras de vida
            this.CargarBarraDeVida();

            return;
        }

        public void ActualizarNombreJugador(string NombreJugador)
        {
            this.NombreJugador = NombreJugador;
        }

        private void CargarBarraDeVida()
        {
            spriteBarraJugador = new TgcSprite(drawerBarras);
            spriteBarraJugador.Texture = TgcTexture.createTexture(MediaDir + "Textures\\Sprites\\barraVacia.png");
            spriteBarraJugador.Position = new Vector2(D3DDevice.Instance.Width - POSICION_X_BARRA_VIDA, POSICION_Y_BARRA_VIDA + (30 * this.MultiplicadorPos));
            spriteBarraJugador.Scaling = new Vector2(0.1f, 0.4f);

            spriteBarraJugadorLlena = new TgcSprite(drawerBarras);
            spriteBarraJugadorLlena.Texture = TgcTexture.createTexture(MediaDir + "Textures\\Sprites\\barraLlena.png");
            spriteBarraJugadorLlena.Position = new Vector2(D3DDevice.Instance.Width - POSICION_X_BARRA_VIDA, POSICION_Y_BARRA_VIDA + (30 * this.MultiplicadorPos));
            spriteBarraJugadorLlena.Scaling = new Vector2(0.1f, 0.4f);
            spriteBarraJugadorLlena.Color = Color.Blue;
        }

        private void DibujarBarraDeVida()
        {
            //Dibujo el nombre del Jugador y de los competidores
            this.letraJugadores.drawText(this.NombreJugador, Convert.ToInt32(D3DDevice.Instance.Width) - POSICION_X_BARRA_VIDA - 125, POSICION_Y_BARRA_VIDA - 12 + (30 * this.MultiplicadorPos), Color.DeepSkyBlue);

            //Calculo la vida de cada auto
            this.spriteBarraJugadorLlena.Scaling = new Vector2(this.cantVidaJugador * 0.001f, 0.4f);

            //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
            this.drawerBarras.beginDrawSprite();

            //Dibujo las barras
            this.spriteBarraJugador.render();
            this.spriteBarraJugadorLlena.render();

            //Finalizar el dibujado de Sprites
            this.drawerBarras.endDrawSprite();
            ///////////////////////            
        }

        public float GetVidaJugador()
        {
            return this.cantVidaJugador;
        }

        public void SetVidaJugador(float modificacion)
        {
            if (this.cantVidaJugador > 0)
                this.cantVidaJugador -= modificacion;
            else
                this.cantVidaJugador = 0;
        }

        public void Update()
        {
            
        }

        public void Render()
        {
            this.DibujarBarraDeVida();
        }

        public void Dispose()
        {
            this.spriteBarraJugador.dispose();
            this.spriteBarraJugadorLlena.dispose();
        }

    }
}
