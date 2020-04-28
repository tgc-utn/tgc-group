using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Torreta : IRenderizable
    {
        private readonly string mediaDir;
        private readonly TGCVector3 posicionInicial;
        private TgcMesh mainMesh;

        private TGCMatrix baseScaleRotation;
        private TGCMatrix baseQuaternionTranslation;
        private float giro = 0f;

        public Torreta(string mediaDir, TGCVector3 posicionInicial)
        {
            this.mediaDir = mediaDir;
            this.posicionInicial = posicionInicial;
        }
        public void Init()
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene2 = loader.loadSceneFromFile(mediaDir + "Xwing\\Turbolaser-TgcScene.xml");

            //Solo nos interesa el primer modelo de esta escena (tiene solo uno)
            mainMesh = scene2.Meshes[0];
            mainMesh.Position = posicionInicial;
            baseQuaternionTranslation = TGCMatrix.Translation(new TGCVector3(0.0f, 0.01f, 0.0f));
            baseScaleRotation = TGCMatrix.Scaling(new TGCVector3(0.15f, 0.15f, 0.15f));
            mainMesh.Transform = TGCMatrix.Scaling(0.1f, 0.1f, 0.1f);
        }

        public void Update(float elapsedTime)
        {
            var rot = new TGCVector3(0,0,0);
            giro += 20f * elapsedTime;
            rot.Y = Geometry.DegreeToRadian(giro);
            TGCQuaternion rotationX = TGCQuaternion.RotationAxis(new TGCVector3(0.0f, 1.0f, 0.0f), rot.Y);

            mainMesh.Transform = baseScaleRotation *
                TGCMatrix.RotationTGCQuaternion(rotationX) *
                baseQuaternionTranslation;
            //TODO logica de giro y disparo hacia la nave.

            //TODO colision con balas y destruccion de la misma
        }


        public void Render()
        {
            mainMesh.Render();
        }
        public void Dispose()
        {
            mainMesh.Dispose();
        }





    }
}
