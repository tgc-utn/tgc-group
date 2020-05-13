using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Laser : IRenderizable
    {
        private readonly string mediaDir;
        private readonly TGCVector3 posicionInicial;
        private readonly TGCVector3 direccion;
        private TgcMesh mainMesh;

        private TGCMatrix baseScaleRotation;
        private TGCMatrix baseQuaternionTranslation;
        public Laser(string mediaDir, TGCVector3 posicionInicial,TGCVector3 direccion)
        {
            this.mediaDir = mediaDir;
            this.posicionInicial = posicionInicial;
            this.direccion = direccion;
        }
        public void Init()
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene2 = loader.loadSceneFromFile(mediaDir + "Xwing\\laser-TgcScene.xml");

            //Solo nos interesa el primer modelo de esta escena (tiene solo uno)
            mainMesh = scene2.Meshes[0];
            mainMesh.Position = posicionInicial;
            baseQuaternionTranslation = TGCMatrix.Translation(new TGCVector3(0.0f, 0.01f, 0.0f));
            baseScaleRotation = TGCMatrix.Scaling(new TGCVector3(.2f, .2f, .2f));
            mainMesh.Transform = TGCMatrix.Scaling(0.1f, 0.1f, 0.1f) * TGCMatrix.Translation(mainMesh.Position);
        }

        public void Update(float elapsedTime)
        {
            TGCQuaternion rotation = TGCQuaternion.RotationAxis(new TGCVector3(1.0f, 0.0f, 0.0f), Geometry.DegreeToRadian(90f));
            var direccionDisparo = direccion;
            direccionDisparo.Normalize();
            var movement = direccionDisparo * 10f * elapsedTime;
            mainMesh.Position = mainMesh.Position + movement;
            var matrizTransformacion = baseScaleRotation * TGCMatrix.RotationTGCQuaternion(rotation)
                * TGCMatrix.Translation(mainMesh.Position);
            mainMesh.Transform = matrizTransformacion;
            mainMesh.BoundingBox.transform(matrizTransformacion);
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

    }
}
