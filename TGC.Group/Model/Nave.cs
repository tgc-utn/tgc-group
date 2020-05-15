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
        private readonly float velocidadBase;
        private float velocidadActual;
        private readonly float aceleracionMovimiento;
        private TGCVector3 rotacionBase;
        private TGCVector3 rotacionActual;
        private readonly float velocidadRotacion;
        

        public Nave(string mediaDir, TGCVector3 posicionInicial, TgcD3dInput input)
        {
            this.mediaDir = mediaDir;
            this.posicion = posicionInicial;
            this.input = input;
            this.velocidadBase = 2f;
            this.velocidadActual = velocidadBase;
            this.aceleracionMovimiento = 0.01f;
            this.rotacionBase = new TGCVector3(0f, Geometry.DegreeToRadian(180f), 0f);
            this.rotacionActual = rotacionBase;
            this.velocidadRotacion = 0.008f;
        }


        public void Init()
        {
            modeloNave = new ModeloCompuesto(mediaDir + "XWing\\X-Wing-TgcScene.xml", posicion);
            TGCVector3 rotacionInicial = new TGCVector3(0f, 1f, 0f) * Geometry.DegreeToRadian(180f);
            modeloNave.CambiarRotacion(rotacionInicial);
        }



        private void MoverseEnDireccion(TGCVector3 versorDirector, float elapsedTime)
        {
            TGCVector3 movimientoDelFrame = new TGCVector3(0, 0, 0);
            TGCVector3 movimientoAdelante = new TGCVector3(0, 0, 1);

            movimientoDelFrame += versorDirector + movimientoAdelante;
            movimientoDelFrame *= 10f * elapsedTime * velocidadActual;

            posicion += movimientoDelFrame;

            modeloNave.CambiarPosicion(posicion);
        }

        public void Update(float elapsedTime)
        { 
            TGCVector3 direccionDelInput = new TGCVector3(0, 0, 0); //A "direccion" se refiere a direccion y sentido.

            
            RotarYMoverseHorizontalmenteSegunInput(ref direccionDelInput);
            RotarYMoverseVerticalmenteSegunInput(ref direccionDelInput);
            AcelerarSegunInput();

            MoverseEnDireccion(direccionDelInput, elapsedTime);
            modeloNave.CambiarRotacion(rotacionActual);

        }

        private void AcelerarSegunInput()
        {
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
        }

        private void RotarYMoverseVerticalmenteSegunInput(ref TGCVector3 direccionDelInput)
        {
            if (input.keyDown(Key.Up) || input.keyDown(Key.W))
            {
                direccionDelInput.Y = 1;
                RotarArriba();
            }
            else if (input.keyDown(Key.Down) || input.keyDown(Key.S))
            {
                direccionDelInput.Y = -1;
                RotarAbajo();
            }
            else
            {
                VolverARotacionVerticalNormal();
            }
        }

        private void RotarYMoverseHorizontalmenteSegunInput(ref TGCVector3 direccionDelInput)
        {
            if (input.keyDown(Key.Left) || input.keyDown(Key.A))
            {
                direccionDelInput.X = -1;
                RotarIzquierda();
            }
            else if (input.keyDown(Key.Right) || input.keyDown(Key.D))
            {
                direccionDelInput.X = 1;
                RotarDerecha();
            }
            else
            {
                VolverARotacionHorizontalNormal();
            }

            
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

        #region Aceleraciones (ifs)
        private void Acelerar()
        {
            float velocidadMaxima = velocidadBase * 4;

            if (velocidadActual < velocidadMaxima)
            {
                velocidadActual += aceleracionMovimiento;
            }
        }

        private void Desacelerar()
        {
            float velocidadMinima = velocidadBase / 2;

            if (velocidadActual > velocidadMinima)
            {
                velocidadActual -= aceleracionMovimiento;
            }
        }

        private void VolverAVelocidadNormal()
        {

            if (velocidadActual != velocidadBase)
            {
                if (velocidadActual > velocidadBase)
                {
                    Desacelerar();
                }
                else
                {
                    Acelerar();
                }
            }
        }
        #endregion

        #region Rotacion (ifs)
        private void RotarDerecha()
        {
            float rotacionMaxima = Geometry.DegreeToRadian(-20f);

            if (rotacionActual.Z > rotacionMaxima)
            {
                rotacionActual += new TGCVector3(0,0,-velocidadRotacion/2);
            }
        }

        private void RotarIzquierda()
        {
            float rotacionMaxima = Geometry.DegreeToRadian(20f);

            if (rotacionActual.Z < rotacionMaxima)
            {
                rotacionActual += new TGCVector3(0, 0, velocidadRotacion/2);
            }
        }

        private void VolverARotacionHorizontalNormal()
        {
            if(rotacionActual.Z != rotacionBase.Z)
            {
                if(rotacionActual.Z > rotacionBase.Z)
                {
                    RotarDerecha();
                }
                else
                {
                    RotarIzquierda();
                }
            }
        }

        private void RotarArriba()
        {
            float rotacionMaxima = Geometry.DegreeToRadian(10f);

            if (rotacionActual.X < rotacionMaxima)
            {
                rotacionActual += new TGCVector3(velocidadRotacion, 0, 0);
            }
        }

        private void RotarAbajo()
        {
            float rotacionMaxima = Geometry.DegreeToRadian(-10f);

            if (rotacionActual.X > rotacionMaxima)
            {
                rotacionActual += new TGCVector3(-velocidadRotacion, 0, 0);
            }
        }

        private void VolverARotacionVerticalNormal()
        {
            if (rotacionActual.X != rotacionBase.X)
            {
                if (rotacionActual.X > rotacionBase.X)
                {
                    RotarAbajo();
                }
                else
                {
                    RotarArriba();
                }
            }
        }

        #endregion

        public TGCVector3 GetPosicion()
        {
            return posicion;
        }

    }
}

