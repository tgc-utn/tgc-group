using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Input;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Text;
using System.Drawing;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using Microsoft.DirectX.Direct3D;

namespace TGC.Group.Model
{
    public class Nave : IRenderizable
    {
        private readonly string mediaDir;
        private TGCVector3 posicion; 
        private ModeloCompuesto modeloNave;
        private readonly TgcD3dInput input;
        private float velocidad = 0.7f;

        public Nave(string mediaDir, TGCVector3 posicionInicial, TgcD3dInput input)
        {
            this.mediaDir = mediaDir;
            this.posicion = posicionInicial;
            this.input = input;
        }


        public void Init()
        {
            modeloNave = new ModeloCompuesto(mediaDir + "XWing\\X-Wing-TgcScene.xml", posicion);
            var rotacionInicial = new TGCVector3(0f, 1f, 0f) * Geometry.DegreeToRadian(180f);
            modeloNave.CambiarRotacion(rotacionInicial);
        }



        private void MoverseEnDireccion(TGCVector3 versorDirector, float elapsedTime)
        {
            TGCVector3 movimientoDelFrame = new TGCVector3(0, 0, 0);
            TGCVector3 movimientoAdelante = new TGCVector3(0, 0, 1);

            movimientoDelFrame += versorDirector + movimientoAdelante;
            movimientoDelFrame *= 10f * elapsedTime * velocidad;

            posicion += movimientoDelFrame;

            modeloNave.CambiarPosicion(posicion);
        }

        private void Acelerar()
        {
            if(velocidad < 4f)
            {
                velocidad += 0.01f;
            }
        }

        private void Desacelerar()
        {
            if(velocidad > 0.5f)
            {
                velocidad -= 0.01f;
            }
        }

        private void VolverAVelocidadNormal()
        {
            if(velocidad != 1f)
            {
                if(velocidad > 1f)
                {
                    Desacelerar();
                }
                else
                {
                    Acelerar();
                }
            }
        }


        public void Update(float elapsedTime)
        {
            TGCVector3 direccionDelInput = new TGCVector3(0, 0, 0); //A "direccion" se refiere a direccion y sentido.
            TGCVector3 giroRotacion = new TGCVector3(0f, Geometry.DegreeToRadian(180f), 0f);

            if (input.keyDown(Key.Left) || input.keyDown(Key.A))
            {
                direccionDelInput.X = -1;
                giroRotacion.Z = Geometry.DegreeToRadian(20f);
            }
            else if (input.keyDown(Key.Right) || input.keyDown(Key.D))
            {
                direccionDelInput.X = 1;
                giroRotacion.Z = Geometry.DegreeToRadian(-20f);
            }
            else
            {
                giroRotacion.Z = Geometry.DegreeToRadian(0f);
            }

            if (input.keyDown(Key.Up) || input.keyDown(Key.W))
            {
                direccionDelInput.Y = 1;
                giroRotacion.X = Geometry.DegreeToRadian(10f);
            }
            else if (input.keyDown(Key.Down) || input.keyDown(Key.S))
            {
                direccionDelInput.Y = -1;
                giroRotacion.X = Geometry.DegreeToRadian(-10f);
            }
            else
            {
                giroRotacion.X = Geometry.DegreeToRadian(0f);
            }

            if (input.keyDown(Key.LeftShift))
            {
                Acelerar();
            }
            else if (input.keyDown(Key.LeftControl))
            {
                Desacelerar();
            }
            else
            {
                VolverAVelocidadNormal();
            }

            MoverseEnDireccion(direccionDelInput, elapsedTime);
            modeloNave.CambiarRotacion(giroRotacion);

        }

        public void Render()
        {
            modeloNave.aplicarTransformaciones();
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

