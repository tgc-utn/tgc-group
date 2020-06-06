using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.Direct3D;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    public class Laser : IRenderizable
    {
        protected string direccionDeScene;
        protected readonly TGCVector3 posicionInicial;
        protected readonly TGCVector3 direccion;
        internal float velocidad;
        protected TGCMatrix baseScaleRotation;
        protected TGCMatrix baseQuaternionTranslation;
        protected TgcMesh mainMesh;
        private DateTime tiempoDeSpawn;

        public Laser(string direccionDeScene, TGCVector3 posicionInicial,TGCVector3 direccion)
        {
            this.direccionDeScene = direccionDeScene;
            this.posicionInicial = posicionInicial;
            this.direccion = direccion;
            this.velocidad = 1;
            this.tiempoDeSpawn = DateTime.Now;
        }

        public void Init()
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene2 = loader.loadSceneFromFile(direccionDeScene);

            //Solo nos interesa el primer modelo de esta escena (tiene solo uno)
            mainMesh = scene2.Meshes[0];
            mainMesh.Position = posicionInicial;
            baseQuaternionTranslation = TGCMatrix.Translation(posicionInicial);
            baseScaleRotation = TGCMatrix.Scaling(new TGCVector3(1f, 1f, 1f));
            TGCQuaternion rotation = TGCQuaternion.RotationAxis(new TGCVector3(1.0f, 0.0f, 0.0f), Geometry.DegreeToRadian(90f));
            mainMesh.Transform = TGCMatrix.Scaling(0.1f, 0.1f, 0.1f) * TGCMatrix.RotationTGCQuaternion(rotation) * TGCMatrix.Translation(mainMesh.Position);
        }

        public virtual void Update(float elapsedTime)
        {
            if (GameManager.Instance.Pause)
                return;

            TGCQuaternion rotation = TGCQuaternion.RotationAxis(new TGCVector3(1.0f, 0.0f, 0.0f), Geometry.DegreeToRadian(90f));
            TGCVector3 direccionDisparo = direccion;
            direccionDisparo.Normalize();
            TGCVector3 movement = direccionDisparo * 60f * elapsedTime * velocidad;
            mainMesh.Position += movement;

            TGCQuaternion giro = QuaternionDireccion(direccionDisparo);
            TGCMatrix matrizTransformacion = baseScaleRotation * TGCMatrix.RotationTGCQuaternion(rotation*giro)
                * TGCMatrix.Translation(mainMesh.Position);
            mainMesh.Transform = matrizTransformacion;
            //mainMesh.updateBoundingBox();
            mainMesh.BoundingBox.transform(matrizTransformacion);

        }

        public Boolean ColisionaConMapa()
        {
            return GameManager.Instance.GetObstaculosMapa().Any(parteMapa => TgcCollisionUtils.testAABBAABB(parteMapa.GetBoundingBox(), this.GetMainMesh().BoundingBox));
        }

        public void Render()
        {
            mainMesh.Render();
            mainMesh.BoundingBox.Render();

        }
        public void Dispose()
        {
            mainMesh.Dispose();
        }

        public virtual TGCQuaternion QuaternionDireccion(TGCVector3 direccionDisparoNormalizado)
        {
            TGCVector3 DireccionA = new TGCVector3(0, 0, -1);
            TGCVector3 cross = TGCVector3.Cross(DireccionA, direccionDisparoNormalizado);
            TGCQuaternion newRotation = TGCQuaternion.RotationAxis(cross, FastMath.Acos(TGCVector3.Dot(DireccionA, direccionDisparoNormalizado)));
            return newRotation;
        }

        public TgcMesh GetMainMesh()
        {
            return mainMesh;
        }

        public Boolean SuperoTiempoDeVida(float tiempoLimite)
        {
            return (DateTime.Now - tiempoDeSpawn).TotalSeconds > tiempoLimite;
        }
    }
}
