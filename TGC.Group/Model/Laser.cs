using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Laser : IRenderizable
    {
        private readonly string mediaDir;
        private readonly TGCVector3 posicionInicial;
        private TgcMesh mainMesh;

        private TGCMatrix baseScaleRotation;
        private TGCMatrix baseQuaternionTranslation;
        public Laser(string mediaDir, TGCVector3 posicionInicial)
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
            baseScaleRotation = TGCMatrix.Scaling(new TGCVector3(1f, 1f, 1f));
            //mainMesh.Transform = TGCMatrix.Scaling(0.1f, 0.1f, 0.1f);
        }

        public void Update(float elapsedTime)
        {
          
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
