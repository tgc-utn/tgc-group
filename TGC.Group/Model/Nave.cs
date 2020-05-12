using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Input;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Text;
using System.Drawing;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace TGC.Group.Model
{
    public class Nave : IRenderizable
    {
        private readonly string mediaDir;
        private TGCVector3 posicion; 
        private ModeloCompuesto modeloNave;
        private readonly TgcD3dInput input;

        public Nave(string mediaDir, TGCVector3 posicionInicial, TgcD3dInput input)
        {
            this.mediaDir = mediaDir;
            this.posicion = posicionInicial;
            this.input = input;
        }

        public void Init()
        {
            modeloNave = new ModeloCompuesto(mediaDir + "XWing\\X-Wing-TgcScene.xml", posicion);
        }



        private void MoverseEnDireccion(TGCVector3 versorDirector, float elapsedTime)
        {
            TGCVector3 movimientoDelFrame = new TGCVector3(0, 0, 0);
            TGCVector3 movimientoAdelante = new TGCVector3(0, 0, 1);

            movimientoDelFrame += versorDirector + movimientoAdelante;
            movimientoDelFrame *= 10f * elapsedTime;

            posicion += movimientoDelFrame;

            modeloNave.CambiarPosicion(posicion);

        }


        public void Update(float elapsedTime)
        {
            TGCVector3 direccionDelInput = new TGCVector3(0, 0, 0); //A "direccion" se refiere a direccion y sentido.

            if (input.keyDown(Key.Left) || input.keyDown(Key.A))
            {
                direccionDelInput.X = -1;
            }
            else if (input.keyDown(Key.Right) || input.keyDown(Key.D))
            {
                direccionDelInput.X = 1;
            }

            if (input.keyDown(Key.Up) || input.keyDown(Key.W))
            {
                direccionDelInput.Y = 1;
            }
            else if (input.keyDown(Key.Down) || input.keyDown(Key.S))
            {
                direccionDelInput.Y = -1;
            }

            MoverseEnDireccion(direccionDelInput, elapsedTime);

        }


        public void Render()
        {
            modeloNave.Render();
            //new TgcText2D().drawText("Posicion de la nave:\n" + posicion.ToString(), 5, 20, Color.White);
            //new TgcText2D().drawText("Rotacion de la nave:\n" + mainMesh.Rotation.ToString(), 5, 100, Color.White);
        }

        public void Dispose()
        {
            modeloNave.Dispose();
        }

        public TGCVector3 GetPosicion()
        {
            return posicion;
        }

    }
}

