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
        protected float velocidadSalto = 70f;
        //maxima altura que puede saltar un vehiculo
        protected float alturaMaximaDeSalto = 50f;
        protected float velocidadMaxima = 300f;
        //tiempo transcurrido desde el ultimo render, se usa para hacer calculos de velocidad
        private float elapsedTime;
        private float transcurrido;
        protected float aceleracionAvance = 0.2f;

        public Vehiculo(string rutaAMesh)
        {
            //inicializo todo
            this.vectorAdelante = new TGCVector3(0, 0, 1);
            this.vectorSalto = new TGCVector3(0, 1, 0);
            this.crearMesh(rutaAMesh);
            this.velocidadActual = 0;
            //para que no se rompa todo hago esto por si nos olvidamos de setearlo
            this.elapsedTime = 0;
            this.transcurrido = 0;
            //stop = new TimeSpan(DateTime.Now.Ticks);
            //Console.WriteLine(stop.Subtract(start).TotalMilliseconds);
        }

        private void crearMesh(string rutaAMesh)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(rutaAMesh);
            this.mesh = scene.Meshes[0];
            //lo roto 180 grados por que sino, el frente queda mirando a la camara
            mesh.RotateY(FastMath.PI);
            //lo escale por que el vehiculo se veia muy chico. pero se podria acercar la camara de ultima
            mesh.Scale = new TGCVector3(0.1f, 0.1f, 0.1f);
        }

        //aca se pudre todo por que todos los moviemientos del vehiculo son con vectores

        public void avanzar()
        {
            //lo que hace Move es sumarle a la posicion del mesh, un vector que le paso
            // ejemplo: suponete que el auto esta ubicado en el (0, 0, 0), si lo quiero
            // desplazar 10 unidades hacia adelante y el vectorAdelante está en (0, 0, 1)
            //que es un versor apuntando para el lado de las z positivas entonces la cuenta
            //seria asi (0, 0, 0) + (0, 0, 1) * 10 * t = (0, 0, 10 * t) teniendo en cuenta
            // que "t" es el tiempo transcurrido. éste último vector es la nueva posicion
            //de mi mesh.
            //Ahora suponete que el vehiculo se encuentra en (35, 0, -6) y el vectorAdelante
            //ésta apuntando al (1, 0, 2) entonces se pone más picante la cosa por que no entendes
            //nada pero en realidad es lo mismo que en el ejemplo anterior: 
            // (35, 0, -6) + (1, 0, 2) * 10 * t = (35 + 10t, 0, -6 + 20t) sabiendo
            //que siempre se desplaza 10 unidades
            if (this.velocidadActual == 0){
                this.iniciarReloj();
            }
            this.velocidadActual = this.velocidadFisica();
            TGCVector3 cuenta = this.vectorAdelante * this.velocidadActual * this.elapsedTime;
            System.Console.WriteLine("({0}, {1}, {2}) * {3} * {4} = ({5}, {6}, {7})", this.vectorAdelante.X, this.vectorAdelante.Y, this.vectorAdelante.Z, this.velocidadActual, this.elapsedTime, cuenta.X, cuenta.Y, cuenta.Z);
            mesh.Move(cuenta);
            
            
        }

        private void iniciarReloj() {
            this.transcurrido += this.elapsedTime * 250;
        }

        private float velocidadFisica()
        {
            System.Console.WriteLine("tiempoTranscurrido: {0}", this.tiempoTranscurrido());
            return System.Math.Min(this.velocidadMaxima, this.velocidadActual + this.aceleracionAvance * this.tiempoTranscurrido());
        }

        private float velocidadFisicaRetroceso()
        {
            return System.Math.Max(-this.velocidadMaxima, this.velocidadActual + (-this.aceleracionAvance) * tiempoTranscurrido());
        }

        private float tiempoTranscurrido()
        {
            return this.transcurrido;
        }

        public void retroceder()
        {
            if (this.velocidadActual == 0)
            {
                this.iniciarReloj();
            }
            //lo mismo que en el anterior nada más que cambio el sentido del vector adelante
            this.velocidadActual = this.velocidadFisicaRetroceso();
            mesh.Move(this.vectorAdelante * this.velocidadActual * this.elapsedTime);
        }

        public void actualizarVelocidad()
        {
            float constanteDesaceleracion = 0.5f;
            if(this.velocidadActual > 0)
            {
                this.velocidadActual -= constanteDesaceleracion;
                this.velocidadActual = (this.velocidadActual < 0) ? 0 : this.velocidadActual;
                mesh.Move(this.vectorAdelante * this.velocidadActual * this.elapsedTime);
            }
            else if(this.velocidadActual < 0)
            {
                this.velocidadActual += constanteDesaceleracion;
                this.velocidadActual = (this.velocidadActual > 0) ? 0 : this.velocidadActual;
                mesh.Move(this.vectorAdelante * this.velocidadActual * this.elapsedTime);
            }
        }

        public void doblarALaDerecha(CamaraEnTerceraPersona camara)
        {
            //aca como se que voy a avanzar girando a la derecha, lo que hago es:
            //1. Calculo la rotacion real, desacoplada del tipo de hardware que usa la pc
            float rotacionReal = this.velocidadRotacion * this.elapsedTime;
            //2. Giro el MESH, es decir, el vehiculo físico
            this.mesh.RotateY(rotacionReal);
            //3. Creo una matrix de rotacion que gira una coordenada a un cierto angulo (en este
            // caso "rotacionReal" calculada previamente)
            TGCMatrix matrizDeRotacion = new TGCMatrix();
            matrizDeRotacion.RotateY(rotacionReal);
            //4. Le aplico la matriz de rotacion al vectorAdelante, para actualizarlo
            vectorAdelante.TransformCoordinate(matrizDeRotacion);
            //5. Roto tambien la camara para que siga mirando a la parte trasera del auto
            camara.rotateY(rotacionReal);

        }

        //lo mismo que arriba
        public void doblarALaIzquierda(CamaraEnTerceraPersona camara)
        {
            float rotacionReal = -this.velocidadRotacion * this.elapsedTime;
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

        //aca viene la parte turbia
        //cuando el usuario apreta por primera vez el boton de saltar, entra por este metodo y realiza la lógica
        //luego hasta que el auto no termine de realizar el salto, por más que el usuario
        //siga apretando espacio, el auto no va a hacer nada (va a finalizar el salto actual antes de volver a saltar).
        public void saltar()
        {
            //verifico si el auto no esta en pleno salto, ni cayendo del mismo
            if(!this.estaSubiendo() && !this.estaBajando())
            {
                //calculo el vector que le tengo que sumar al vector posicion del vehiculo para realizar la
                //transformacion
                TGCVector3 nuevaPosicion = this.vectorSalto * this.velocidadSalto * this.elapsedTime;
                //transformo
                mesh.Move(OperacionesConVectores.minimaAlturaEntreVectores(new TGCVector3(0, alturaMaximaDeSalto, 0), nuevaPosicion));
                
            }
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
                    this.velocidadSalto = 70f;
                    this.cambiarDireccionVectorSalto();
                }
                
            }
        }

        private float velocidadFisicaDeSalto()
        {
            if(this.velocidadSalto < 1)
            {
                
                return this.velocidadSalto = 1;
            }
            return System.Math.Max(this.velocidadSalto /= 1.001f, 2f);

        }

        private float velocidadFisicaDeCaida()
        {
            return this.velocidadSalto *= 1.005f;
        }

        //esta funcion si no la invoco, el ejemplo funciona igual
        //por las dudas la dejo aca
        /*
        public void transformar()
        {
            mesh.Transform =
                TGCMatrix.Scaling(mesh.Scale)
                            * TGCMatrix.RotationYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z)
                            * TGCMatrix.Translation(mesh.Position);
            
        }
        */

        private float distanciaDelPiso()
        {
            return mesh.Position.Y;
        }

        public TGCVector3 getVectorAdelante()
        {
            return this.vectorAdelante;
        }

        public void setElapsedTime(float time)
        {
            TgcText2D texto = new TgcText2D();
            string dialogo = "Velocidad = {0}km";
            string.Format(dialogo, this.velocidadActual);
            texto.drawText(dialogo, 10, 30, Color.Red);
            texto.render();
            this.elapsedTime = time;

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
