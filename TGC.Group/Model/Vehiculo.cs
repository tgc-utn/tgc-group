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
    class Vehiculo
    {   
        private TgcMesh mesh;
        private TGCVector3 vectorAdelante;
        private const float CONSTANTE_ROTACION = 1f;
        private const float CONSTANTE_VELOCIDAD = 150f;

        public Vehiculo(string rutaAMesh)
        {
            vectorAdelante = new TGCVector3(0, 0, 1);
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(rutaAMesh);
            this.mesh = scene.Meshes[0];
        }

        public void avanzar(float tiempoTranscurrido)
        {
            mesh.Move(this.vectorAdelante * CONSTANTE_VELOCIDAD * tiempoTranscurrido);
        }

        public void retroceder(float tiempoTranscurrido)
        {
            mesh.Move(-(this.vectorAdelante * CONSTANTE_VELOCIDAD * tiempoTranscurrido));
        }

        public void avanzarHaciaLaDerecha(float tiempoTranscurrido)
        {
            this.avanzar(tiempoTranscurrido);
            this.rotarEnY(CONSTANTE_ROTACION * tiempoTranscurrido);
            TGCMatrix matrizDeRotacion = new TGCMatrix();
            matrizDeRotacion.RotateY(CONSTANTE_ROTACION * tiempoTranscurrido);
            vectorAdelante.TransformCoordinate(matrizDeRotacion);

        }

        public void retrocederHaciaLaDerecha(float tiempoTranscurrido)
        {
            this.retroceder(tiempoTranscurrido);
            this.rotarEnY(-CONSTANTE_ROTACION * tiempoTranscurrido);
            TGCMatrix matrizDeRotacion = new TGCMatrix();
            matrizDeRotacion.RotateY(-CONSTANTE_ROTACION * tiempoTranscurrido);
            vectorAdelante.TransformCoordinate(matrizDeRotacion);

        }

        public void retrocederHaciaLaIzquierda(float tiempoTranscurrido)
        {
            this.retroceder(tiempoTranscurrido);
            this.rotarEnY(CONSTANTE_ROTACION * tiempoTranscurrido);
            TGCMatrix matrizDeRotacion = new TGCMatrix();
            matrizDeRotacion.RotateY(CONSTANTE_ROTACION * tiempoTranscurrido);
            vectorAdelante.TransformCoordinate(matrizDeRotacion);

        }

        public void avanzarHaciaLaIzquierda(float tiempoTranscurrido)
        {
            this.avanzar(tiempoTranscurrido);
            this.rotarEnY(-CONSTANTE_ROTACION * tiempoTranscurrido);
            TGCMatrix matrizDeRotacion = new TGCMatrix();
            matrizDeRotacion.RotateY(-CONSTANTE_ROTACION * tiempoTranscurrido);
            vectorAdelante.TransformCoordinate(matrizDeRotacion);

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

        public void saltar()
        {
            return;
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


    }
}
