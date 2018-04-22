using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Text;
using System.Drawing;
using Microsoft.DirectX.DirectInput;
using System.Timers;
using System.Diagnostics;

namespace TGC.Group.Model
{
    abstract class Vehiculo
    {
        //las velocidades actuales se van usar despues para modelar un mejor movimiento del aut0
        //pd: ahora no se usan
        private float velocidadActual;
        private float velocidadActualDeSalto;
        //utilizo un mesh como atributo para abstraernos de que es una sopa de triangulos
        //de esta forma, le mandamos mensajes al vehiculo como (avanzar, retroceder, etc)
        //y este se encarga de utilizar el mesh
        private TgcMesh mesh;
        //lo necesito para saber la direccion en que mira el frente del auto
        private TGCVector3 vectorAdelante;
        //lo necesito para saber si el auto esta saltando o esta cayendo del salto
        private TGCVector3 vectorSalto;
        //velocidades de rotacion, traslado y salto, en las clases hijas se modifican segun
        // el tipo de vehiculo (camion, auto, etc)
        protected float velocidadRotacion = 1f;
        protected float velocidadMaximaDeSalto = 60f;
        //maxima altura que puede saltar un vehiculo
        protected float alturaMaximaDeSalto = 50f;
        protected float velocidadMaxima = 300f;
        //tiempo transcurrido desde el ultimo render, se usa para hacer calculos de velocidad
        private float elapsedTime;
        private float deltaTiempo;
        private float deltaTiempoSalto;
        protected float aceleracionAvance = 0.5f;
        protected float aceleracionRetroceso = 0.3f;
        private float aceleracionGravedad = 9.81f;

        public Vehiculo(string rutaAMesh)
        {
            //inicializo todo
            this.vectorAdelante = new TGCVector3(0, 0, 1);
            this.vectorSalto = new TGCVector3(0, 1, 0);
            this.crearMesh(rutaAMesh);
            this.velocidadActual = 0;
            this.velocidadActualDeSalto = 0;
            //para que no se rompa todo hago esto por si nos olvidamos de setearlo
            this.elapsedTime = 0;
            this.deltaTiempo = 0;
            this.deltaTiempoSalto = 0;
        }

        private void crearMesh(string rutaAMesh)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(rutaAMesh);
            this.mesh = scene.Meshes[0];
            mesh.RotateY(FastMath.PI);
            mesh.Scale = new TGCVector3(0.1f, 0.1f, 0.1f);
        }

        public void avanzar()
        {
            this.avanzarTiempo();
            this.velocidadActual = this.velocidadFisica();
            TGCVector3 cuenta = this.vectorAdelante * this.velocidadActual * this.elapsedTime;
            mesh.Move(cuenta);      
        }

        private float velocidadFisica()
        {
            return System.Math.Min(this.velocidadMaxima, this.velocidadActual + this.aceleracionAvance * this.tiempoTranscurrido());
        }

        private float velocidadFisicaRetroceso()
        {
            return System.Math.Max(-this.velocidadMaxima, this.velocidadActual + (-this.aceleracionRetroceso) * this.tiempoTranscurrido());
        }

        private float tiempoTranscurrido()
        {
            return this.deltaTiempo;
        }

        public void retroceder()
        {
            this.avanzarTiempo();
            //lo mismo que en el anterior nada más que cambio el sentido del vector adelante
            this.velocidadActual = this.velocidadFisicaRetroceso();
            mesh.Move(this.vectorAdelante * this.velocidadActual * this.elapsedTime);
        }

        public void actualizarVelocidad()
        {
            float constanteDesaceleracion = 0.5f;
            this.resetearDeltaTiempo();
            if (this.velocidadActual > 0)
            {
                this.velocidadActual -= constanteDesaceleracion;
                if(this.velocidadActual < 0)
                {
                    this.velocidadActual = 0;
                    this.resetearDeltaTiempo();
                }
                mesh.Move(this.vectorAdelante * this.velocidadActual * this.elapsedTime);
            }
            else if(this.velocidadActual < 0)
            {  
                this.velocidadActual += constanteDesaceleracion;
                if (this.velocidadActual > 0)
                {
                    this.velocidadActual = 0;
                    this.resetearDeltaTiempo();
                }
                mesh.Move(this.vectorAdelante * this.velocidadActual * this.elapsedTime);
            }
        }

        private void  resetearDeltaTiempo()
        {
            this.deltaTiempo = 0;
        }

        public float getDeltaTiempo()
        {
            return this.deltaTiempo;
        }

        public void doblarALaDerecha(CamaraEnTerceraPersona camara)
        {
            if (this.velocidadActual == 0) return;
            float rotacionReal = this.velocidadRotacion * this.elapsedTime;
            rotacionReal = (this.velocidadActual > 0) ? rotacionReal : -rotacionReal;
            this.mesh.RotateY(rotacionReal);
            TGCMatrix matrizDeRotacion = new TGCMatrix();
            matrizDeRotacion.RotateY(rotacionReal);
            vectorAdelante.TransformCoordinate(matrizDeRotacion);
            camara.rotateY(rotacionReal);

        }

        //lo mismo que arriba
        public void doblarALaIzquierda(CamaraEnTerceraPersona camara)
        {
            if (this.velocidadActual == 0) return;
            float rotacionReal = -this.velocidadRotacion * this.elapsedTime;
            rotacionReal = (this.velocidadActual < 0) ? -rotacionReal : rotacionReal;
            this.mesh.RotateY(rotacionReal);
            TGCMatrix matrizDeRotacion = new TGCMatrix();
            matrizDeRotacion.RotateY(rotacionReal);
            vectorAdelante.TransformCoordinate(matrizDeRotacion);
            camara.rotateY(rotacionReal);

        }

        //escala el auto, tal vez la saqeu por que no sirve mucho
        public void escalar(TGCVector3 vector)
        {
            mesh.Scale = vector;
        }

        //devuelve la posicion del auto en el mapa, sirve bastante
        public TGCVector3 posicion()
        {
            return mesh.Position;
        }

        private void iniciarRelojSalto()
        {
            this.deltaTiempoSalto = 0;
        }

        //aca viene la parte turbia
        //cuando el usuario apreta por primera vez el boton de saltar, entra por este metodo y realiza la lógica
        //luego hasta que el auto no termine de realizar el salto, por más que el usuario
        //siga apretando espacio, el auto no va a hacer nada (va a finalizar el salto actual antes de volver a saltar).
        public void saltar()
        {
            //verifico si el auto no esta en pleno salto, ni cayendo del mismo
            if(!this.estaSubiendo() && !this.estaBajando())
            {
                this.iniciarRelojSalto();
                //calculo el vector que le tengo que sumar al vector posicion del vehiculo para realizar la
                //transformacion
                TGCVector3 nuevaPosicion = this.vectorSalto * this.velocidadFisicaDeSalto() * this.elapsedTime;
                //transformo
                mesh.Move(OperacionesConVectores.minimaAlturaEntreVectores(new TGCVector3(0, alturaMaximaDeSalto, 0), nuevaPosicion));
                
            }
        }

        private float tiempoTranscurridoSalto()
        {
            return this.deltaTiempoSalto;
        }

        public float velocidadFisicaDeSalto()
        {
            return System.Math.Min(this.velocidadMaximaDeSalto, this.velocidadActualDeSalto + this.aceleracionGravedad * this.tiempoTranscurridoSalto());
        }

        public float velocidadFisicaDeCaida()
        {
            return 0;
            //return System.Math.Min(this.velocidadMaximaDeSalto, this.velocidadActualDeSalto + this.aceleracionGravedad * this.tiempoTranscurridoSalto());
        }

        private void cambiarDireccionVectorSalto()
        {
            this.vectorSalto.Y = -this.vectorSalto.Y;
        }

        public bool estaSubiendo()
        {
            return this.distanciaDelPiso() > 0 && this.vectorSalto.Y > 0;
        }

        public bool estaBajando()
        {
            return this.distanciaDelPiso() > 0 && this.vectorSalto.Y < 0;
        }

        public void actualizarSalto()
        {
            //si el auto esta en pleno salto
            if (this.estaSubiendo())
            {
                TGCVector3 nuevaPosicion = this.vectorSalto * this.velocidadFisicaDeSalto() * this.elapsedTime;
                nuevaPosicion = (mesh.Position.Y + nuevaPosicion.Y) > alturaMaximaDeSalto? new TGCVector3(0, alturaMaximaDeSalto - mesh.Position.Y, 0) : nuevaPosicion;
                mesh.Move(nuevaPosicion);
                if (mesh.Position.Y == alturaMaximaDeSalto)
                {
                    this.cambiarDireccionVectorSalto();
                }    
                
            }
            //lo mismo que arriba
            else if(this.estaBajando())
            {
                TGCVector3 nuevaPosicion = this.vectorSalto * this.velocidadFisicaDeCaida() * this.elapsedTime;
                nuevaPosicion = (mesh.Position.Y + nuevaPosicion.Y) < 0 ? new TGCVector3(0, -mesh.Position.Y, 0) : nuevaPosicion;
                mesh.Move(nuevaPosicion);
                if (mesh.Position.Y == 0)
                {
                    //this.velocidadSalto = 70f;
                    this.cambiarDireccionVectorSalto();
                }
                
            }
        }

        private float distanciaDelPiso()
        {
            return mesh.Position.Y;
        }

        public TGCVector3 getVectorAdelante()
        {
            return this.vectorAdelante;
        }

        private void avanzarTiempo()
        {
            this.deltaTiempo += this.elapsedTime;
        }

        public void setElapsedTime(float time)
        {
            this.elapsedTime = time;
            if(this.deltaTiempo != 0)
            {
                this.avanzarTiempo();
            }

        }

        public float getVelocidadActual()
        {
            return this.velocidadActual;
        }

        public void Render()
        {
            mesh.Render();
        }

        public void dispose()
        {
            mesh.Dispose();
        }

    }
}
