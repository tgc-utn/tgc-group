using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.UtilsGroup;

namespace TGC.Group.Model
{
    class HUDMenu
    {
        private string MediaDir;
        private TgcDrawText letraMenu;
        private TgcDrawText letraTitulo;
        private int NroMenu = 1;
        private int PosicionMenu = 1;
        //I: inicio
        //S: Salir
        //C: Controles
        //E: Ejecutar
        private string EstadoMenu;
        private string NombreJugador = "";

        public HUDMenu(string MediaDir)
        {
            this.MediaDir = MediaDir;
            this.letraMenu = new TgcDrawText(D3DDevice.Instance.Device, "Rock it", 18, MediaDir);
            this.letraTitulo = new TgcDrawText(D3DDevice.Instance.Device, "Rock it", 36, MediaDir);
            this.EstadoMenu = "I";
        }

        private void DibujarTitulo()
        {
            this.letraTitulo.drawText("TWISTED CHANO", (Convert.ToInt32(D3DDevice.Instance.Width) / 2) - 250, (Convert.ToInt32(D3DDevice.Instance.Height) / 2) - 300, Color.DeepSkyBlue);
        }

        public void SetPosicionMenu(int Movimiento)
        {
            if (((this.PosicionMenu + Movimiento) < 1) || ((this.PosicionMenu + Movimiento) > 3))
                return;

            this.PosicionMenu += Movimiento;
        }

        public string GetEstadoMenu ()
        {
            return EstadoMenu;
        }

        public void SetNroMenu(int NroMenu)
        {
            this.NroMenu = NroMenu;

            if (NroMenu == 1)
                this.EstadoMenu = "I";

            if (NroMenu == 2)
                this.EstadoMenu = "E";

            this.NombreJugador = "";
        }

        public string GetNombreJugador()
        {
            return this.NombreJugador;
        }

        public void EscribirTeclasEnPantalla(TgcD3dInput Input)
        {
            if (this.EstadoMenu == "E")
            {
                if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.BackSpace))
                {
                    if (this.NombreJugador.Length == 0)
                        return;

                    this.NombreJugador = this.NombreJugador.Substring(0, this.NombreJugador.Length - 1);
                }

                if (this.NombreJugador.Length < 10)
                {
                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.A))
                    {
                        this.NombreJugador += "a";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.B))
                    {
                        this.NombreJugador += "b";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.C))
                    {
                        this.NombreJugador += "c";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.D))
                    {
                        this.NombreJugador += "d";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.E))
                    {
                        this.NombreJugador += "e";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.F))
                    {
                        this.NombreJugador += "f";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.G))
                    {
                        this.NombreJugador += "g";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.H))
                    {
                        this.NombreJugador += "h";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.I))
                    {
                        this.NombreJugador += "i";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.J))
                    {
                        this.NombreJugador += "j";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.K))
                    {
                        this.NombreJugador += "k";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.L))
                    {
                        this.NombreJugador += "l";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.M))
                    {
                        this.NombreJugador += "m";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.N))
                    {
                        this.NombreJugador += "n";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.O))
                    {
                        this.NombreJugador += "o";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.P))
                    {
                        this.NombreJugador += "p";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.Q))
                    {
                        this.NombreJugador += "q";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.R))
                    {
                        this.NombreJugador += "r";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.S))
                    {
                        this.NombreJugador += "s";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.T))
                    {
                        this.NombreJugador += "t";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.U))
                    {
                        this.NombreJugador += "u";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.V))
                    {
                        this.NombreJugador += "v";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.W))
                    {
                        this.NombreJugador += "w";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.X))
                    {
                        this.NombreJugador += "x";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.Y))
                    {
                        this.NombreJugador += "y";
                    }

                    if (Input.keyPressed(Microsoft.DirectX.DirectInput.Key.Z))
                    {
                        this.NombreJugador += "z";
                    }
                }
            }
        }

        private void DibujarMenu()
        {
            Color colorPosicion1 = Color.Black;
            Color colorPosicion2 = Color.Black;
            Color colorPosicion3 = Color.Black;

            if (this.NroMenu == 1)
            {
                if (PosicionMenu == 1)
                {
                    colorPosicion1 = Color.Red;
                    this.EstadoMenu = "I";
                }

                if (PosicionMenu == 2)
                {
                    colorPosicion2 = Color.Red;
                    this.EstadoMenu = "C";

                    this.letraMenu.drawText("Acelerar/Frenar:", (Convert.ToInt32(D3DDevice.Instance.Width) / 2) - 350, (Convert.ToInt32(D3DDevice.Instance.Height) / 2) + 125, colorPosicion2);
                    this.letraMenu.drawText("Up/Down", (Convert.ToInt32(D3DDevice.Instance.Width) / 2) - 50, (Convert.ToInt32(D3DDevice.Instance.Height) / 2) + 125, colorPosicion2);
                    this.letraMenu.drawText("Saltar:", (Convert.ToInt32(D3DDevice.Instance.Width) / 2) - 350, (Convert.ToInt32(D3DDevice.Instance.Height) / 2) + 200, colorPosicion2);
                    this.letraMenu.drawText("Space", (Convert.ToInt32(D3DDevice.Instance.Width) / 2) - 50, (Convert.ToInt32(D3DDevice.Instance.Height) / 2) + 200, colorPosicion2);
                }

                if (PosicionMenu == 3)
                {
                    colorPosicion3 = Color.Red;
                    this.EstadoMenu = "S";
                }

                this.letraMenu.drawText("INICIO", (Convert.ToInt32(D3DDevice.Instance.Width) / 2) - 50, (Convert.ToInt32(D3DDevice.Instance.Height) / 2) - 100, colorPosicion1);
                this.letraMenu.drawText("CONTROLES", (Convert.ToInt32(D3DDevice.Instance.Width) / 2) - 50, (Convert.ToInt32(D3DDevice.Instance.Height) / 2) - 25, colorPosicion2);
                this.letraMenu.drawText("SALIR", (Convert.ToInt32(D3DDevice.Instance.Width) / 2) - 50, (Convert.ToInt32(D3DDevice.Instance.Height) / 2) + 50, colorPosicion3);
            }

            if (this.NroMenu == 2)
            {
                this.letraMenu.drawText("Ingrese su nombre y presione Enter", (Convert.ToInt32(D3DDevice.Instance.Width) / 2) - 325, (Convert.ToInt32(D3DDevice.Instance.Height) / 2) - 100, Color.Red);
                this.letraMenu.drawText(this.NombreJugador, (Convert.ToInt32(D3DDevice.Instance.Width) / 2) - 100, (Convert.ToInt32(D3DDevice.Instance.Height) / 2) - 25, Color.DeepSkyBlue);
            }
        }

        public void Render()
        {
            this.DibujarTitulo();
            this.DibujarMenu();
        }
    }
}
