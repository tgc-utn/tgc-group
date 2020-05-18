using TGC.Core.Mathematica;
using TGC.Core.Text;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Collision;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TGC.Group.Model
{
    public class Nave : IRenderizable
    {
        private TGCVector3 posicion; 
        private ModeloCompuesto modeloNave;
        private readonly InputDelJugador input;
        private readonly float velocidadBase;
        private float velocidadActual;
        private readonly float aceleracionMovimiento;
        private TGCVector3 rotacionBase;
        private TGCVector3 rotacionActual;
        private readonly float velocidadRotacion;
        

        public Nave(string mediaDir, TGCVector3 posicionInicial, InputDelJugador input)
        {
            this.modeloNave = new ModeloCompuesto(mediaDir + "XWing\\X-Wing-TgcScene.xml", posicion);
            this.posicion = posicionInicial;
            this.input = input;
            this.velocidadBase = 5f;
            this.velocidadActual = velocidadBase;
            this.aceleracionMovimiento = 0.01f;
            this.rotacionBase = new TGCVector3(0f, Geometry.DegreeToRadian(180f), 0f);
            this.rotacionActual = rotacionBase;
            this.velocidadRotacion = 0.008f;
        }


        public void Init()
        {
            
            TGCVector3 rotacionInicial = new TGCVector3(0f, 1f, 0f) * Geometry.DegreeToRadian(180f);
            modeloNave.CambiarRotacion(rotacionInicial);
        }

        public void Update(float elapsedTime)
        {

            if (input.HayInputDeAceleracion())
            {
                float aceleracionDelInput = input.SentidoDeAceleracionDelInput() * aceleracionMovimiento;
                Acelerar(aceleracionDelInput);
            }
            else
            {
                VolverAVelocidadNormal();
            }

            if (input.HayInputDeRotacion())
            {
                RotarEnDireccion(input.RotacionDelInput());
            }
            else
            {
                VolverARotacionNormal();
            }

            MoverseEnDireccion(input.DireccionDelInput(),elapsedTime);


        }
        public void Render()
        {
            modeloNave.aplicarTransformaciones();
            modeloNave.Render();
            new TgcText2D().drawText("Velocidad de la nave: " + velocidadActual.ToString(), 5, 20, Color.White);
            new TgcText2D().drawText("Tocando un Laser:\n"+ naveEstaColisionandoConLaser().ToString(), 5, 40, Color.White);
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

        private void MoverseEnDireccion(TGCVector3 versorDirector,float elapsedTime)
        {
            TGCVector3 movimientoDelFrame = new TGCVector3(0, 0, 0);
            TGCVector3 movimientoAdelante = new TGCVector3(0, 0, 1);

            movimientoDelFrame += versorDirector + movimientoAdelante;
            movimientoDelFrame *= 10f * elapsedTime * velocidadActual;

            posicion += movimientoDelFrame;

            modeloNave.CambiarPosicion(posicion);
        }
        private void RotarEnDireccion(TGCVector3 versorDirector)
        {
            float limiteEnZ = Geometry.DegreeToRadian(20f);
            float limiteEnX = Geometry.DegreeToRadian(15f);
            TGCVector3 rotacionASumar = versorDirector * velocidadRotacion;

            float nuevaRotacionZ = NuevaRotacionEnEjeSegunLimite(rotacionASumar.Z + rotacionActual.Z, limiteEnZ);
            float nuevaRotacionX = NuevaRotacionEnEjeSegunLimite(rotacionASumar.X + rotacionActual.X, limiteEnX);

            rotacionActual = new TGCVector3(nuevaRotacionX, Geometry.DegreeToRadian(180f), nuevaRotacionZ);
            modeloNave.CambiarRotacion(rotacionActual);
        }
        private void Acelerar(float aceleracion)
        {
            float velocidadMaxima = velocidadBase * 4;
            float velocidadMinima = velocidadBase / 2;

            float nuevaVelocidad = aceleracion + velocidadActual;

            if (nuevaVelocidad < velocidadMinima)
            {
                velocidadActual = velocidadMinima;
            }
            else if(nuevaVelocidad > velocidadMaxima)
            {
                velocidadActual = velocidadMaxima;
            }
            else
            {
                velocidadActual = nuevaVelocidad;
            }
        }

        private void VolverARotacionNormal()
        {
            TGCVector3 direccionDeRotacionNecesaria = TGCVector3.Normalize(rotacionBase - rotacionActual);
            RotarEnDireccion(direccionDeRotacionNecesaria);
        }
        private void VolverAVelocidadNormal()
        {
            float aceleracion = Math.Sign(velocidadBase - velocidadActual) * aceleracionMovimiento;
            Acelerar(aceleracion);
        }
        private float NuevaRotacionEnEjeSegunLimite(float nuevaPosibleRotacion, float rotacionLimite) //Mal nombre aaa
        {
            float rotacionMaxima = rotacionLimite;
            float rotacionMinima = -rotacionLimite;

            if (nuevaPosibleRotacion >= rotacionMaxima)
            {
                return rotacionMaxima;
            }
            else if (nuevaPosibleRotacion <= rotacionMinima)
            {
                return rotacionMinima;
            }
            else
            {
                return nuevaPosibleRotacion;
            }
        }
        private bool naveEstaColisionandoConLaser()
        {
            var listaLaseres = GameManager.Instance.obtenerLaseres();
            return listaLaseres.Any(laser => modeloNave.colisionaConLaser(laser));
        }

    }
}

