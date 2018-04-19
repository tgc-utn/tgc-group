using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Example;

namespace TGC.Group.Model
{
    abstract class Vehiculo
    {   
        //utilizo un mesh como atributo para abstraernos de que es una sopa de triangulos
        //de esta forma, le mandamos mensajes al vehiculo como (avanzar, retroceder, etc)
        //y este se encarga de utilizar el mesh
        private TgcMesh mesh;
        //lo necesito para saber la direccion en que mira el frente del auto
        private TGCVector3 vectorAdelante;
        //velocidades de rotacion, traslado y salto, en las clases hijas se modifican segun
        // el tipo de vehiculo
        protected float velocidadRotacion = 1f;
        protected float velocidadTraslado = 150f;
        protected float velocidadSalto = 70f;
        //las utilizo para saber si el auto esta en pleno salto o cayendo del mismo
        //proximamente esto va a ser reemplazado por un vector
        private bool subiendo, bajando;
        //maxima altura que puede saltar un vehiculo
        private const float ALTURA_SALTO = 30f;
        //tiempo transcurrido desde el ultimo render, se usa para hacer calculos de velocidad
        private float elapsedTime;

        public Vehiculo(string rutaAMesh)
        {
            //inicializo todo
            vectorAdelante = new TGCVector3(0, 0, 1);
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(rutaAMesh);
            this.mesh = scene.Meshes[0];
            subiendo = false;
            bajando = false;
        }

        //aca se pudre todo por que todos los moviemientos del vehiculo son con vectores

        public void avanzar()
        {
            //lo que hace Move es sumarle a la posicion del mesh, un vector que le paso
            // ejemplo: suponete que el auto esta ubicado en el (0, 0, 0), si lo quiero
            // desplazar 10 unidades hacia adelante y el vectorAdelante está en (0, 0, 1)
            //que es un versor apuntando para el lado de las z positivas entonces la cuenta
            //seria asi (0, 0, 0) + (0, 0, 1) = (0, 0, 1). éste último vector es la nueva posicion
            //de mi mesh. Obviamente hice el ejemplo sin multiplicarle la velocidad del vehiculo
            //(velocidad de traslado) y el tiempo transcurrido para que no sea mas complejo
            //Ahora suponete que el vehiculo se encuentra en (35, 0, -6) y el vectorAdelante
            //ésta apuntando al (1, 0, 1) entonces se pone más picante la cosa por que no entendes
            //nada pero en realidad es lo mismo que en el ejemplo anterior: (35, 0, -6) + (1, 0, 1) = (36, 0, -5)
            mesh.Move(this.vectorAdelante * this.velocidadTraslado * this.elapsedTime);
        }

        public void retroceder()
        {
            //lo mismo que en el anterior nada más que cambio el sentido del vector adelante
            mesh.Move(-(this.vectorAdelante * this.velocidadTraslado * this.elapsedTime));
        }

        public void avanzarHaciaLaDerecha(CamaraEnTerceraPersona camara)
        {
            //aca como se que voy a avanzar girando a la derecha, lo que hago es:
            //1. Avanzo comunmente como si hiria para adelante, esto lo que va a hacer es
            //avanzar una distancia muy chica
            float rotacionReal = this.velocidadRotacion * this.elapsedTime;
            this.avanzar();
            //2. Giro el MESH, es decir, el vehiculo físico
            this.rotarEnY(rotacionReal);
            //3. Creo una matrix de rotacion que gira una coordenada a un cierto angulo (en este
            // caso "rotacionReal" calculada previamente)
            TGCMatrix matrizDeRotacion = new TGCMatrix();
            matrizDeRotacion.RotateY(rotacionReal);
            //4. Le aplico la matriz de rotacion al vectorAdelante, para actualizarlo
            vectorAdelante.TransformCoordinate(matrizDeRotacion);
            //5. Roto tambien la camara para que siga mirando a la parte trasera del auto
            camara.rotateY(rotacionReal);


        }

        public void retrocederHaciaLaDerecha(CamaraEnTerceraPersona camara)
        {
            //lo mismo que antes
            float rotacionReal = -this.velocidadRotacion * this.elapsedTime;
            this.retroceder();
            this.rotarEnY(rotacionReal);
            TGCMatrix matrizDeRotacion = new TGCMatrix();
            matrizDeRotacion.RotateY(rotacionReal);
            vectorAdelante.TransformCoordinate(matrizDeRotacion);
            camara.rotateY(rotacionReal);

        }

        public void retrocederHaciaLaIzquierda(CamaraEnTerceraPersona camara)
        {
            //lo mismo que antes
            float rotacionReal = this.velocidadRotacion * this.elapsedTime;
            this.retroceder();
            this.rotarEnY(rotacionReal);
            TGCMatrix matrizDeRotacion = new TGCMatrix();
            matrizDeRotacion.RotateY(rotacionReal);
            vectorAdelante.TransformCoordinate(matrizDeRotacion);
            camara.rotateY(rotacionReal);

        }

        public void avanzarHaciaLaIzquierda(CamaraEnTerceraPersona camara)
        {
            //lo mismo que antes
            float rotacionReal = -this.velocidadRotacion * this.elapsedTime;
            this.avanzar();
            this.rotarEnY(rotacionReal);
            TGCMatrix matrizDeRotacion = new TGCMatrix();
            matrizDeRotacion.RotateY(rotacionReal);
            vectorAdelante.TransformCoordinate(matrizDeRotacion);
            camara.rotateY(rotacionReal);

        }

        //estas son funciones para rotar el mesh, tal vez las saque por que no sirven mucho

        public void rotarEnX(float anguloEnRadianes)
        {
            mesh.RotateX(anguloEnRadianes);
        }

        public void rotarEnY(float anguloEnRadianes)
        {
            mesh.RotateY(anguloEnRadianes);
        }

        public void rotarEnZ(float anguloEnRadianes)
        {
            mesh.RotateZ(anguloEnRadianes);
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
        //luego hasta que el auto no ascienda y descienda hasta llegar nuevamente al piso, por más que el usuario
        //siga apretando espacio, el auto no va a hacer nada.
        public void saltar()
        {
            //verifico si el auto no esta en pleno salto, ni cayendo del mismo
            if(!subiendo && !bajando)
            {
                //calculo el vector que le tengo que sumar al vector posicion del vehiculo para realizar la
                //transformacion
                TGCVector3 nuevaPosicion = new TGCVector3(0, 1, 0) * this.velocidadSalto * this.elapsedTime;
                //transformo
                mesh.Move(this.minimaAlturaEntreVectores(new TGCVector3(0, ALTURA_SALTO, 0), nuevaPosicion));
                //ahora indico que el auto esta en pleno salto
                this.estaSubiendo();
            }
        }

        public void estaSubiendo()
        {
            this.subiendo = true;
            this.bajando = false;
        }

        public void estaBajando()
        {
            this.subiendo = false;
            this.bajando = true;
        }

        public void terminoElSalto()
        {
            this.subiendo = false;
            this.bajando = false;
        }

        public void actualizarSalto()
        {
            //si el auto esta en pleno salto
            if (this.subiendo)
            {
                //calcula el nuevo desplazamiento hacia arriba que tiene que realizar
                TGCVector3 nuevaPosicion = new TGCVector3(0, 1, 0) * this.velocidadSalto * this.elapsedTime;
                //si ese desplazamiento es mayor al permitido (ALTURA_SALTO) entonces solo salta hasta la altura maxima
                //caso contrario, desplazate hacia arriba lo que tenias pensado desplazar
                nuevaPosicion = (mesh.Position.Y + nuevaPosicion.Y) > ALTURA_SALTO? new TGCVector3(0, ALTURA_SALTO - mesh.Position.Y, 0) : nuevaPosicion;
                //transformacion
                mesh.Move(nuevaPosicion);
                //si llegue a la altura maxima, indicar que el auto esta bajando
                if (mesh.Position.Y == ALTURA_SALTO)
                {
                    this.estaBajando();
                }    
                
            }
            //lo mismo que arriba
            else if(bajando)
            {
                TGCVector3 nuevaPosicion = new TGCVector3(0, -1, 0) * this.velocidadSalto * this.elapsedTime;
                nuevaPosicion = (mesh.Position.Y + nuevaPosicion.Y) < 0 ? new TGCVector3(0, -mesh.Position.Y, 0) : nuevaPosicion;
                mesh.Move(nuevaPosicion);
                if (mesh.Position.Y == 0)
                {
                    this.terminoElSalto();
                }
                
            }
        }

        //funciones para usar en el Render y Dispose
        public void transformar()
        {
            mesh.Transform =
                TGCMatrix.Scaling(mesh.Scale)
                            * TGCMatrix.RotationYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z)
                            * TGCMatrix.Translation(mesh.Position);
            
        }

        public void setElapsedTime(float time)
        {
            this.elapsedTime = time;
        }

        public void Render()
        {
            mesh.Render();
        }

        public void dispose()
        {
            mesh.Dispose();
        }

        //funciones de ayuda, deberian ir en otra clase
        public TGCVector3 minimaAlturaEntreVectores(TGCVector3 vector1, TGCVector3 vector2)
        {
            return vector1.Y < vector2.Y ? vector1 : vector2;
        }

        public TGCVector3 maximaAlturaEntreVectores(TGCVector3 vector1, TGCVector3 vector2)
        {
            return vector1.Y > vector2.Y ? vector1 : vector2;
        }

        public TGCVector3 sumaDeVectores(TGCVector3 vector1, TGCVector3 vector2)
        {
            return new TGCVector3(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z);
        }

    }
}
