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
        private TgcMesh mesh;
        private TGCVector3 vectorAdelante;
        protected float velocidadRotacion = 1f;
        protected float velocidadTraslado = 150f;
        protected float velocidadSalto = 70f;
        private bool subiendo, bajando;
        private const float ALTURA_SALTO = 30f;

        public Vehiculo(string rutaAMesh)
        {
            vectorAdelante = new TGCVector3(0, 0, 1);
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(rutaAMesh);
            this.mesh = scene.Meshes[0];
            subiendo = false;
            bajando = false;
        }

        public void avanzar(float tiempoTranscurrido)
        {
            mesh.Move(this.vectorAdelante * this.velocidadTraslado * tiempoTranscurrido);
        }

        public void retroceder(float tiempoTranscurrido)
        {
            mesh.Move(-(this.vectorAdelante * this.velocidadTraslado * tiempoTranscurrido));
        }

        public void avanzarHaciaLaDerecha(float tiempoTranscurrido, CamaraEnTerceraPersona camara)
        {
            float rotacionReal = this.velocidadRotacion * tiempoTranscurrido;
            this.avanzar(tiempoTranscurrido);
            this.rotarEnY(rotacionReal);
            TGCMatrix matrizDeRotacion = new TGCMatrix();
            matrizDeRotacion.RotateY(rotacionReal);
            vectorAdelante.TransformCoordinate(matrizDeRotacion);
            camara.rotateY(rotacionReal);


        }

        public void retrocederHaciaLaDerecha(float tiempoTranscurrido, CamaraEnTerceraPersona camara)
        {
            float rotacionReal = -this.velocidadRotacion * tiempoTranscurrido;
            this.retroceder(tiempoTranscurrido);
            this.rotarEnY(rotacionReal);
            TGCMatrix matrizDeRotacion = new TGCMatrix();
            matrizDeRotacion.RotateY(rotacionReal);
            vectorAdelante.TransformCoordinate(matrizDeRotacion);
            camara.rotateY(rotacionReal);

        }

        public void retrocederHaciaLaIzquierda(float tiempoTranscurrido, CamaraEnTerceraPersona camara)
        {
            float rotacionReal = this.velocidadRotacion * tiempoTranscurrido;
            this.retroceder(tiempoTranscurrido);
            this.rotarEnY(rotacionReal);
            TGCMatrix matrizDeRotacion = new TGCMatrix();
            matrizDeRotacion.RotateY(rotacionReal);
            vectorAdelante.TransformCoordinate(matrizDeRotacion);
            camara.rotateY(rotacionReal);

        }

        public void avanzarHaciaLaIzquierda(float tiempoTranscurrido, CamaraEnTerceraPersona camara)
        {
            float rotacionReal = -this.velocidadRotacion * tiempoTranscurrido;
            this.avanzar(tiempoTranscurrido);
            this.rotarEnY(rotacionReal);
            TGCMatrix matrizDeRotacion = new TGCMatrix();
            matrizDeRotacion.RotateY(rotacionReal);
            vectorAdelante.TransformCoordinate(matrizDeRotacion);
            camara.rotateY(rotacionReal);

        }

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

        public void escalar(TGCVector3 vector)
        {
            mesh.Scale = vector;
        }

        public TGCVector3 posicion()
        {
            return mesh.Position;
        }

        public void saltar(float tiempoTranscurrido)
        {
            if(!subiendo && !bajando)
            {
                TGCVector3 nuevaPosicion = new TGCVector3(0, 1, 0) * this.velocidadSalto * tiempoTranscurrido;
                mesh.Move(this.minimaAlturaEntreVectores(new TGCVector3(0, ALTURA_SALTO, 0), nuevaPosicion));
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

        public void actualizarSalto(float tiempoTranscurrido)
        {
            if (this.subiendo)
            {
                TGCVector3 nuevaPosicion = new TGCVector3(0, 1, 0) * this.velocidadSalto * tiempoTranscurrido;
                nuevaPosicion = (mesh.Position.Y + nuevaPosicion.Y) > ALTURA_SALTO? new TGCVector3(0, ALTURA_SALTO - mesh.Position.Y, 0) : nuevaPosicion;
                mesh.Move(nuevaPosicion);
                if (mesh.Position.Y == ALTURA_SALTO)
                {
                    this.estaBajando();
                }    
                
            }
            else if(bajando)
            {
                TGCVector3 nuevaPosicion = new TGCVector3(0, -1, 0) * this.velocidadSalto * tiempoTranscurrido;
                nuevaPosicion = (mesh.Position.Y + nuevaPosicion.Y) < 0 ? new TGCVector3(0, -mesh.Position.Y, 0) : nuevaPosicion;
                mesh.Move(nuevaPosicion);
                if (mesh.Position.Y == 0)
                {
                    this.terminoElSalto();
                }
                
            }
        }

        public void transformar()
        {
            mesh.Transform =
                TGCMatrix.Scaling(mesh.Scale)
                            * TGCMatrix.RotationYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z)
                            * TGCMatrix.Translation(mesh.Position);
            
        }

        public void Render()
        {
            mesh.Render();
        }

        public void dispose()
        {
            mesh.Dispose();
        }

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
