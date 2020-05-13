using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Bloque:IRenderizable
    {
        private readonly string mediaDir;
        private readonly TGCVector3 posicionInicial;

        private TGCMatrix ScaleMatrix;
        private TGCMatrix TranslationMatrix;
        private TgcScene Scene;
        private String nombreMapa;

        public Bloque(string mediaDir, TGCVector3 posicionInicial,String nombreMapa)
        {
            this.mediaDir = mediaDir;
            this.posicionInicial = posicionInicial;
            this.nombreMapa = nombreMapa;
        }
        public void Init()
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            Scene = new TgcSceneLoader().loadSceneFromFile(mediaDir + nombreMapa);
            ScaleMatrix = TGCMatrix.Scaling(20f, 20f, 20f);
            TranslationMatrix = TGCMatrix.Translation(posicionInicial);

        }

        public void Update(float elapsedTime)
        {
            Scene.Meshes.ForEach(delegate (TgcMesh mesh) { mesh.Transform = ScaleMatrix*TranslationMatrix; });
        }

        public void Render()
        {
            Scene.RenderAll();


        }
        public void Dispose()
        {
            Scene.DisposeAll();
        }
    }
}
